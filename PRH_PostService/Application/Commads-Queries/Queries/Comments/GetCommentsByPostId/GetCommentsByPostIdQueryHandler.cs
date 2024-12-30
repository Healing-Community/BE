using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.Comments.GetCommentsByPostId
{
    public class GetCommentsByPostIdQueryHandler(
        ICommentRepository commentRepository,
        ICommentTreeService commentTreeService)
        : IRequestHandler<GetCommentsByPostIdQuery, BaseResponse<List<CommentDtoResponse>>>
    {
        public async Task<BaseResponse<List<CommentDtoResponse>>> Handle(GetCommentsByPostIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<CommentDtoResponse>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy tất cả comment của PostId
                var comments = await commentRepository.GetAllCommentsByPostIdAsync(request.PostId);

                if (!comments.Any())
                {
                    response.Success = false;
                    response.Message = "Không có bình luận nào được tìm thấy.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Chuyển đổi sang DTO
                var commentDtos = comments.Select(c => new CommentDtoResponse
                {
                    CommentId = c.CommentId,
                    PostId = c.PostId,
                    ParentId = c.ParentId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CoverImgUrl = c.CoverImgUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Replies = new List<CommentDtoResponse>() // Bắt đầu với danh sách rỗng
                }).ToList();

                // Xây dựng cây bình luận bằng CommentTreeService
                var commentTree = commentTreeService.BuildCommentTree(commentDtos);

                response.Data = commentTree;
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
