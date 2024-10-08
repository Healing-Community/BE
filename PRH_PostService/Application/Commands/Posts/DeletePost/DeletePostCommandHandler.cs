using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Timestamp = DateTime.UtcNow
            };
            try
            {
                await postRepository.DeleteAsync(request.Id);
                response.Success = true;
                response.Message = "Post deleted successfully";
                response.Errors = Enumerable.Empty<string>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to delete post";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }
    }
}
