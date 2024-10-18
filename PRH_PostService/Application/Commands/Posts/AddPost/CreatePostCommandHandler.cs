using Application.Commons;
using Application.Commons.Request;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;


namespace Application.Commands.Posts.AddPost
{
    public class CreatePostCommandHandler(IMessagePublisher messagePublisher, IPostRepository postRepository)
        : IRequestHandler<CreatePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
            if (userId == null)
            {
                response.Success = false;
                response.Message = "Unauthorized";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return response;
            }
            var userGuid = Guid.Parse(userId);
            var post = new Post
            {
                Id = NewId.NextSequentialGuid(),
                UserId = userGuid,
                CategoryId = request.PostDto.CategoryId,
                Title = request.PostDto.Title,
                CoverImgUrl = request.PostDto.CoverImgUrl,
                VideoUrl = request.PostDto.VideoUrl,
                Description = request.PostDto.Description,
                Status = request.PostDto.Status,
                CreateAt = DateTime.UtcNow,
            };
            try
            {
                await postRepository.Create(post);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Post created successfully";
                // Send the Request to the Queue for processing
                var postingRequestCreatedMessage = new PostingRequestCreatedMessage
                {
                    PostingRequestId = NewId.NextSequentialGuid(),
                    UserId = post.UserId,
                    Tittle = post.Title,
                    PostedDate = post.CreateAt
                };
                await messagePublisher.PublishAsync(postingRequestCreatedMessage, QueueName.PostQueue, cancellationToken);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to create post";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
