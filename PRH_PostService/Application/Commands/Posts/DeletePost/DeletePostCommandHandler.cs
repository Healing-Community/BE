using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Posts.DeletePost
{
    public class DeletePostCommandHandler(IPostRepository postRepository) : IRequestHandler<DeletePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await postRepository.DeleteAsync(request.Id);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Post deleted successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to delete post";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
