using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.Comments.GetComments
{
    public class GetCommentsQueryHandler(ICommentRepository commentRepository)
        : IRequestHandler<GetCommentsQuery, BaseResponse<IEnumerable<CommentDtoResponse>>>
    {
        public async Task<BaseResponse<IEnumerable<CommentDtoResponse>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<CommentDtoResponse>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var comments = await commentRepository.GetsAsync();

                // Map Comments to CommentDtoResponse to avoid circular references
                var mappedComments = comments.Select(c => MapToDto(c)).ToList();

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = mappedComments;
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                response.Success = false;
            }
            return response;
        }
        private CommentDtoResponse MapToDto(Comment comment)
        {
            var dto = new CommentDtoResponse
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                ParentId = comment.ParentId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                CoverImgUrl = comment.CoverImgUrl
            };

            dto.Replies = comment.Replies?.Select(MapToDto).ToList() ?? new List<CommentDtoResponse>();
            return dto;
        }
    }
}
