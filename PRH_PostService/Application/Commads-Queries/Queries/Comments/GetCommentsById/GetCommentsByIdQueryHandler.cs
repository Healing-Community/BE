using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Application.Commons.DTOs;
using Application.Interfaces.Service;
using Application.Services;

namespace Application.Queries.Comments.GetCommentsById
{
    public class GetCommentsByIdQueryHandler(
        ICommentRepository commentRepository, 
        ICommentTreeService commentTreeService)
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
                // Lấy tất cả comments liên quan đến commentId (gồm cả replies)
                var comments = await commentRepository.GetAllCommentsByCommentIdAsync(request.Id);

                if (!comments.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bình luận.";
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

                response.Data = commentTree.FirstOrDefault(); // Trả về cây bình luận từ gốc
                response.Success = true;
                response.Message = "Lấy dữ liệu thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
