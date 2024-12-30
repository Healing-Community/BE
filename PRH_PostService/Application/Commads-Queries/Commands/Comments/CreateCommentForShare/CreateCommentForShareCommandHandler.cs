using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Domain.Entities;
using MediatR;
using NUlid;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Application.Commads_Queries.Commands.Comments.CreateCommentForShare
{
    public class CreateCommentForShareCommandHandler : IRequestHandler<CreateCommentForShareCommand, BaseResponse<CommentDtoResponse>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentTreeService _commentTreeService;

        public CreateCommentForShareCommandHandler(ICommentRepository commentRepository, ICommentTreeService commentTreeService)
        {
            _commentRepository = commentRepository;
            _commentTreeService = commentTreeService;
        }

        public async Task<BaseResponse<CommentDtoResponse>> Handle(CreateCommentForShareCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<CommentDtoResponse>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Validate user authentication
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Create new comment
                var newComment = new Comment
                {
                    CommentId = Ulid.NewUlid().ToString(),
                    ShareId = request.CommentDto.ShareId,
                    ParentId = request.CommentDto.ParentId,
                    UserId = userId,
                    Content = request.CommentDto.Content,
                    CoverImgUrl = request.CommentDto.CoverImgUrl,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _commentRepository.Create(newComment);

                // Fetch all comments for the same ShareId to rebuild the tree
                var allComments = await _commentRepository.GetQueryable()
                    .Where(c => c.ShareId == request.CommentDto.ShareId)
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentDtoResponse
                    {
                        CommentId = c.CommentId,
                        ShareId = c.ShareId,
                        ParentId = c.ParentId,
                        Content = c.Content,
                        CoverImgUrl = c.CoverImgUrl,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        Replies = new List<CommentDtoResponse>() // Initialize empty list for replies
                    })
                    .ToListAsync(cancellationToken);

                var commentTree = _commentTreeService.BuildCommentTree(allComments);

                // Find the newly created comment within the rebuilt comment tree
                var createdCommentWithTree = commentTree.FirstOrDefault(c => c.CommentId == newComment.CommentId);

                // Prepare response
                response.Data = createdCommentWithTree ?? new CommentDtoResponse
                {
                    CommentId = newComment.CommentId,
                    ShareId = newComment.ShareId,
                    ParentId = newComment.ParentId,
                    Content = newComment.Content,
                    CoverImgUrl = newComment.CoverImgUrl,
                    CreatedAt = newComment.CreatedAt,
                    UpdatedAt = newComment.UpdatedAt,
                    Replies = new List<CommentDtoResponse>()
                };

                response.Success = true;
                response.Message = "Tạo bình luận thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi !!! Không thể tạo bình luận.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }

}
