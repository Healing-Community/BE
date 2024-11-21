using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.Posts.GetPostsByUserId
{
    public class GetPostsByUserIdQueryHandler(IPostRepository postRepository)
        : IRequestHandler<GetPostsByUserIdQuery, BaseResponse<IEnumerable<PostDto>>>
    {
        public async Task<BaseResponse<IEnumerable<PostDto>>> Handle(GetPostsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<PostDto>>
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                // Lấy danh sách bài viết theo UserId
                var posts = await postRepository.GetByUserIdAsync(request.UserId);

                if (!posts.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bài viết nào.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Data = posts.Select(post => new PostDto
                {
                    PostId = post.PostId,         
                    UserId = post.UserId,         
                    GroupId = post.GroupId,        
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    VideoUrl = post.VideoUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,   
                    UpdateAt = post.UpdateAt    
                });
                response.Success = true;
                response.Message = "Lấy bài viết thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
