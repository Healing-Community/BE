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
                    response.Message = "Không tìm thấy";
                    response.Errors.Add("Không tìm thấy dữ liệu nào có ID được cung cấp.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = comment;
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
