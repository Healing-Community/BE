using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Net;
using UserInformation;


namespace Application.Commads_Queries.Queries.Comments.GetCommentsByShareId
{
    public class GetCommentsByShareIdQueryHandler : IRequestHandler<GetCommentsByShareIdQuery, BaseResponse<List<CommentDtoResponse>>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentTreeService _commentTreeService;
        private readonly IGrpcHelper _grpcHelper;

        public GetCommentsByShareIdQueryHandler(ICommentRepository commentRepository, ICommentTreeService commentTreeService, IGrpcHelper grpcHelper)
        {
            _commentRepository = commentRepository;
            _commentTreeService = commentTreeService;
            _grpcHelper = grpcHelper;
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
                // Fetch comments by ShareId
                var comments = await _commentRepository.GetQueryable()
                    .Where(c => c.ShareId == request.ShareId)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync(cancellationToken);

                // Check if no comments exist
                if (!comments.Any())
                {
                    response.Data = new List<CommentDtoResponse>();
                    response.Success = true;
                    response.Message = "Không có bình luận nào cho ShareId này.";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return response;
                }

                // Convert comments to DTOs and fetch user info via gRPC
                var commentDtos = new List<CommentDtoResponse>();
                foreach (var comment in comments)
                {
                    // Fetch user information via gRPC for each comment's userId
                    var userReply = await _grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                        "UserService",
                        async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = comment.UserId })
                    );

                    // Map comment to DTO
                    commentDtos.Add(new CommentDtoResponse
                    {
                        CommentId = comment.CommentId,
                        PostId = comment.PostId,
                        ShareId = comment.ShareId,
                        ParentId = comment.ParentId,
                        UserId = comment.UserId,
                        Content = comment.Content,
                        CoverImgUrl = comment.CoverImgUrl,
                        CreatedAt = comment.CreatedAt,
                        UpdatedAt = comment.UpdatedAt,
                        ProfilePicture = userReply.ProfilePicture, // Map ProfilePicture
                        UserName = userReply.UserName,           // Map UserName
                        Replies = new List<CommentDtoResponse>() // Initialize Replies as an empty list
                    });
                }

                var commentTree = _commentTreeService.BuildCommentTree(commentDtos);

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
