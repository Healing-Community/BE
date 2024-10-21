using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.Posts.GetPostsById
{
    public class GetPostsByIdQueryHandler(IPostRepository postRepository) : IRequestHandler<GetPostsByIdQuery, BaseResponse<Post>>
    {
        public async Task<BaseResponse<Post>> Handle(GetPostsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Post>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var post = await postRepository.GetByIdAsync(request.id);
                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy";
                    response.Errors.Add("Không tìm thấy dữ liệu nào có ID được cung cấp.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = post;
                response.Success = true;
                response.Message = "Lấy dữ liệu thành công";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Có lỗi xảy ra";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
