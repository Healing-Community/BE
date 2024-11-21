using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Queries.Comments.GetCommentsByPostId
{
    public class GetCommentsByPostIdQueryHandler(ICommentRepository commentRepository)
        : IRequestHandler<GetCommentsByPostIdQuery, BaseResponse<List<CommentDtoResponse>>>
    {
        public async Task<BaseResponse<List<CommentDtoResponse>>> Handle(GetCommentsByPostIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<CommentDtoResponse>>
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                // Lấy danh sách comments theo PostId
                var comments = await commentRepository.GetQueryable()
                    .Where(c => c.PostId == request.PostId)
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
                            Replies = null // Lồng sâu tối đa 1 cấp
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);

                response.Data = comments;
                response.Success = true;
                response.Message = "Lấy danh sách comments theo PostId thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi lấy danh sách comments.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
