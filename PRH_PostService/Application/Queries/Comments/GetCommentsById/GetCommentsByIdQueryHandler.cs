using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Application.Commons.DTOs;

namespace Application.Queries.Comments.GetCommentsById
{
    public class GetCommentsByIdQueryHandler(ICommentRepository commentRepository)
        : IRequestHandler<GetCommentsByIdQuery, BaseResponse<CommentDtoResponse>>
    {
        public async Task<BaseResponse<CommentDtoResponse>> Handle(GetCommentsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<CommentDtoResponse>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                // Truy vấn comment với các phản hồi của nó, nếu có
                var comment = await commentRepository
                    .GetQueryable()
                    .Where(c => c.CommentId == request.Id)
                    .Select(c => new CommentDtoResponse
                    {
                        CommentId = c.CommentId,
                        PostId = c.PostId,
                        ParentId = c.ParentId,
                        UserId = c.UserId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        Replies = c.Replies.Select(r => new CommentDtoResponse
                        {
                            CommentId = r.CommentId,
                            PostId = r.PostId,
                            ParentId = r.ParentId,
                            UserId = r.UserId,
                            Content = r.Content,
                            CreatedAt = r.CreatedAt,
                            UpdatedAt = r.UpdatedAt,
                            Replies = r.Replies.Select(rr => new CommentDtoResponse
                            {
                                CommentId = rr.CommentId,
                                PostId = rr.PostId,
                                ParentId = rr.ParentId,
                                UserId = rr.UserId,
                                Content = rr.Content,
                                CreatedAt = rr.CreatedAt,
                                UpdatedAt = rr.UpdatedAt,
                                Replies = new List<CommentDtoResponse>() // Dừng ở mức sâu nhất
                            }).ToList()
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (comment == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bình luận";
                    response.Errors.Add("Không tìm thấy dữ liệu nào có ID được cung cấp.");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Set response data với comment và phản hồi của nó
                response.Data = comment;
                response.StatusCode = (int)HttpStatusCode.OK;
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
