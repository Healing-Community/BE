using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.Comments.GetComments
{
    public class GetCommentsQueryHandler(ICommentRepository commentRepository)
        : IRequestHandler<GetCommentsQuery, BaseResponse<IEnumerable<Comment>>>
    {
        public async Task<BaseResponse<IEnumerable<Comment>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Comment>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var comments = await commentRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = comments;
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
