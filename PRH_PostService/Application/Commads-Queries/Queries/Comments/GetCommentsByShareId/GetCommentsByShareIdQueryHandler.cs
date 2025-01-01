using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Net;


namespace Application.Commads_Queries.Queries.Comments.GetCommentsByShareId
{
    public class GetCommentsByShareIdQueryHandler : IRequestHandler<GetCommentsByShareIdQuery, BaseResponse<List<CommentDtoResponse>>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentTreeService _commentTreeService;

        public GetCommentsByShareIdQueryHandler(ICommentRepository commentRepository, ICommentTreeService commentTreeService)
        {
            _commentRepository = commentRepository;
            _commentTreeService = commentTreeService;
        }

        public async Task<BaseResponse<List<CommentDtoResponse>>> Handle(GetCommentsByShareIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<CommentDtoResponse>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy danh sách comment theo ShareId
                var comments = await _commentRepository.GetQueryable()
                    .Where(c => c.ShareId == request.ShareId)
                    .OrderBy(c => c.CreatedAt) // Sắp xếp theo thời gian tạo
                    .Select(c => new CommentDtoResponse
                    {
                        CommentId = c.CommentId,
                        PostId = c.PostId,
                        ShareId = c.ShareId,
                        ParentId = c.ParentId,
                        UserId = c.UserId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        CoverImgUrl = c.CoverImgUrl,
                        Replies = new List<CommentDtoResponse>()
                    })
                    .ToListAsync(cancellationToken);

                // Kiểm tra nếu không có bình luận
                if (!comments.Any())
                {
                    response.Data = new List<CommentDtoResponse>();
                    response.Success = true;
                    response.Message = "Không có bình luận nào cho ShareId này.";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return response;
                }

                var commentTree = _commentTreeService.BuildCommentTree(comments);

                response.Data = commentTree;
                response.Success = true;
                response.Message = "Lấy danh sách comment theo ShareId thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi lấy danh sách comment.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }

    }
}
