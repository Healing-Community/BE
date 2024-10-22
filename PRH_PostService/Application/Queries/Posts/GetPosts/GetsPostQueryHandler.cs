using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.Posts.GetPosts
{
    public class GetsPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetsPostQuery, BaseResponse<IEnumerable<Post>>>
    {
        public async Task<BaseResponse<IEnumerable<Post>>> Handle(GetsPostQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Post>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var posts = await postRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = posts;
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
