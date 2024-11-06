using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;


namespace Application.Commands.Posts.DeletePost
{
    public class DeletePostCommandHandler(IPostRepository postRepository) : IRequestHandler<DeletePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                await postRepository.DeleteAsync(request.Id);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Xoá bài viết thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Xoá bài viết thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
