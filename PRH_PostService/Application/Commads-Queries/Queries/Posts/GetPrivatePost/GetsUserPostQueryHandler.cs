using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.Posts.GetPrivatePost;
/// <summary>
/// Get Private Posts Only 
/// </summary>
public class GetsUserPostQueryHandler : IRequestHandler<GetsUserPostQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    private readonly IPostRepository _repository;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public GetsUserPostQueryHandler(IPostRepository repository, IHttpContextAccessor? httpContextAccessor = null)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetsUserPostQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<IEnumerable<PostRecommendDto>>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7),
            Errors = new List<string>()
        };

        try
        {
            // Lấy UserId từ Claims trong HttpContext
            var currentUserId = _httpContextAccessor.HttpContext?.User
                .Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Không có quyền truy cập, chưa đăng nhập hoặc phiên làm việc hết hạn.";
                response.Success = false;
                return response;
            }

            // Kiểm tra quyền truy cập và lọc bài viết dựa trên điều kiện
            var posts = request.UserId == currentUserId
                ? await _repository.GetsPostByPropertyPagingAsync(
                    p => p.UserId == request.UserId && (p.Status == 0 || p.Status == 1),
                    request.PageNumber,
                    request.PageSize
                  )
                : await _repository.GetsPostByPropertyPagingAsync(
                    p => p.UserId == request.UserId && p.Status == 0,
                    request.PageNumber,
                    request.PageSize
                  );

            // Map DTO
            var data = posts.Select(post => new PostRecommendDto
            {
                PostId = post.PostId,
                UserId = post.UserId,
                CategoryId = post.CategoryId,
                Title = post.Title,
                CoverImgUrl = post.CoverImgUrl,
                Description = post.Description,
                Status = post.Status,
                CreateAt = post.CreateAt,
                UpdateAt = post.UpdateAt
            });

            response.Data = data;
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Lấy bài viết thành công.";
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Message = "Đã xảy ra lỗi.";
            response.Errors.Add(ex.Message);
            response.Success = false;
        }

        return response;
    }
}
