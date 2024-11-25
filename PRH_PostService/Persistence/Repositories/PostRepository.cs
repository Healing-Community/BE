using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;


namespace Persistence.Repositories
{
    public class PostRepository(HFDBPostserviceContext context) : IPostRepository
    {
        public async Task Create(Post entity)
        {
            await context.Posts.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.PostId == id);
            if (post == null) return;
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string postId)
        {
            return await context.Posts.AnyAsync(p => p.PostId == postId);
        }

        public async Task<Post> GetByIdAsync(string id)
        {
            return await context.Posts.FirstAsync(x => x.PostId == id);
        }

        public async Task<Post> GetByPropertyAsync(Expression<Func<Post, bool>> predicate)
        {
            var post = await context.Posts.AsNoTracking().FirstOrDefaultAsync(predicate);
            return post ?? new Post() { PostId = Ulid.Empty.ToString() };
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
        {
            return await context.Posts.Where(post => post.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetsAsync()
        {
            return await context.Posts.ToListAsync();
        }

        public async Task Update(string id, Post entity)
        {
            var existingPost = await context.Posts.FirstOrDefaultAsync(x => x.PostId == id);
            if (existingPost == null) return;
            context.Entry(existingPost).CurrentValues.SetValues(entity);
            context.Entry(existingPost).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Post>> GetRecommendedPostsAsync(string userId, int pageNumber, int pageSize)
        {
            // Lấy danh sách CategoryId mà user yêu thích
            var preferredCategories = await context.UserPreferences
                .Where(up => up.UserId == userId)
                .Select(up => up.CategoryId)
                .ToListAsync();

            if (preferredCategories.Any())
            {
                // Lấy bài viết theo danh mục yêu thích
                var preferredPosts = await context.Posts
                    .Where(post => preferredCategories.Contains(post.CategoryId))
                    .OrderBy(_ => Guid.NewGuid()) // Trộn ngẫu nhiên
                    .ToListAsync();

                // Lấy bài viết không thuộc danh mục yêu thích
                var nonPreferredPosts = await context.Posts
                    .Where(post => !preferredCategories.Contains(post.CategoryId))
                    .OrderBy(_ => Guid.NewGuid()) // Trộn ngẫu nhiên
                    .ToListAsync();

                // Ghép bài viết yêu thích lên trước, bài viết không yêu thích xuống sau
                var allPosts = preferredPosts.Concat(nonPreferredPosts);

                // Áp dụng paging
                return allPosts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                // Không có danh mục yêu thích, lấy toàn bộ bài viết và trộn ngẫu nhiên
                var allPosts = await context.Posts
                    .OrderBy(_ => Guid.NewGuid())
                    .ToListAsync();

                // Áp dụng paging
                return allPosts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }
        }

    }
}
