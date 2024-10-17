using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Comments.GetComments
{
    public class GetCommentsQueryHandler(ICommentRepository commentRepository)
        : IRequestHandler<GetCommentsQuery, BaseResponse<IEnumerable<Comment>>>
    {
        public async Task<BaseResponse<IEnumerable<Comment>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Comment>>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var comments = await commentRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Comment retrieved successfully";
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
