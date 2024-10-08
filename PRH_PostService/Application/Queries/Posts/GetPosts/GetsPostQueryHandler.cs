using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;

namespace Application.Queries.Posts.GetPosts
{
    public class GetsPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetsPostQuery, BaseResponse<IEnumerable<Post>>>
    {
        public async Task<BaseResponse<IEnumerable<Post>>> Handle(GetsPostQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Post>>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                var posts = await postRepository.GetsAsync();
                response.Message = "Post retrieved successfully";
                response.Success = true;
                response.Data = posts;
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
