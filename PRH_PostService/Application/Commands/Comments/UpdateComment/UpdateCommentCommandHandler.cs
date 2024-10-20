using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Commands.Comments.UpdateComment
{
    public class UpdateCommentCommandHandler(ICommentRepository commentRepository)
        : IRequestHandler<UpdateCommentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.commentId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var existingComment = await commentRepository.GetByIdAsync(request.commentId);
                var updatedComment = new Comment
                {
                    CommentId = request.commentId,
                    Content = request.CommentDto.Content,
                    UpdatedAt = DateTime.UtcNow,
                };
                await commentRepository.Update(request.commentId, updatedComment);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Cập nhật bình luận thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Không cập nhật được bình luận";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
