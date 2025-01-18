using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Application.Interfaces.Services;
using MediatR;
using NUlid;
using System.Net;
using UserInformation;

namespace Application.Queries.Comments.GetCommentsByPostId
{
    public class GetCommentsByPostIdQueryHandler(
        ICommentRepository commentRepository,
        ICommentTreeService commentTreeService,
        IGrpcHelper grpcHelper)
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

                // Convert comments to DTOs
                var commentDtos = new List<CommentDtoResponse>();

                foreach (var comment in comments)
                {
                    // Fetch user information via gRPC for each comment's userId
                    var userReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                        "UserService",
                        async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = comment.UserId })
                    );

                    // Map to DTO
                    commentDtos.Add(new CommentDtoResponse
                    {
                        CommentId = comment.CommentId,
                        PostId = comment.PostId,
                        ParentId = comment.ParentId,
                        UserId = comment.UserId,
                        Content = comment.Content,
                        CoverImgUrl = comment.CoverImgUrl,
                        ProfilePicture = userReply.ProfilePicture, 
                        UserName = userReply.UserName,
                        CreatedAt = comment.CreatedAt,
                        UpdatedAt = comment.UpdatedAt,
                        Replies = new List<CommentDtoResponse>() // Initialize with an empty list
                    });
                }

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
