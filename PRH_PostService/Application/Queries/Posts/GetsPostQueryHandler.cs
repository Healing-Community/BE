using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;

namespace Application.Queries.Posts
{
    public class GetsPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetsPostQuery, BaseResponse<IEnumerable<Post>>>
    {
        public async Task<BaseResponse<IEnumerable<Post>>> Handle(GetsPostQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Post>>(); 
            response.Id = NewId.NextSequentialGuid();
            response.Timestamp = DateTime.Now;

            var posts = await postRepository.GetsAsync();
            response.Data = posts;

            return response;
        }
    }
}
