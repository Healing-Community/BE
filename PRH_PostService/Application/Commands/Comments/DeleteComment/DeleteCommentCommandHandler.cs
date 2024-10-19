using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;

namespace Application.Commands.Comments.DeleteComment
{
    public class DeleteCommentCommandHandler(ICommentRepository commentRepository)
        : IRequestHandler<DeleteCommentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await commentRepository.DeleteAsync(request.Id);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Xoá bình luận thành công!";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Không xoá được bình luận";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
