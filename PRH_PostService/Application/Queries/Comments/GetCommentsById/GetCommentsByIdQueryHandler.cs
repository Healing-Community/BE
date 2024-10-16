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

namespace Application.Queries.Comments.GetCommentsById
{
    public class GetCommentsByIdQueryHandler(ICommentRepository commentRepository)
        : IRequestHandler<GetCommentsByIdQuery, BaseResponse<Comment>>
    {
        public async Task<BaseResponse<Comment>> Handle(GetCommentsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Comment>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var comment = await commentRepository.GetByIdAsync(request.Id);
                if (comment == null)
                {
                    response.Success = false;
                    response.Message = "Comment not found";
                    response.Errors.Add("No comment found with the provided ID.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = comment;
                response.Success = true;
                response.Message = "Comment retrieved successfully";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
