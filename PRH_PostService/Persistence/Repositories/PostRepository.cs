using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
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

        public async Task<Post?> GetByPropertyAsync(Expression<Func<Post, bool>> predicate)
        {
            var post = await context.Posts.AsNoTracking().FirstOrDefaultAsync(predicate);
            return post;
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
        public async Task<IEnumerable<Post>> GetRecommendedPostsAsync(string? userId, int pageNumber, int pageSize)
        {
            // Nếu userId là rỗng, trả về bài viết ngẫu nhiên
            if (string.IsNullOrEmpty(userId))
            {
                var randomPosts = await context.Posts
                    .Where(p => p.Status == 0)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return randomPosts;
            }

            // Lấy danh sách CategoryId mà user yêu thích
            var preferredCategories = await context.UserPreferences
                .Where(up => up.UserId == userId)
                .Select(up => up.CategoryId)
                .ToListAsync();

            if (preferredCategories.Any())
            {
                
                // Lấy bài viết không thuộc danh mục yêu thích
                // Ghép bài viết yêu thích lên trước, bài viết không yêu thích xuống sau
                var posts = await context.Posts.Where(p => p.Status == 0)
                    .OrderByDescending(p => preferredCategories.Contains(p.CategoryId ?? ""))
                    .ThenByDescending(p => p.CreateAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                return posts;
            }
            return await GetRandomPostsAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Post>> GetRandomPostsAsync(int pageNumber, int pageSize)
        {
            var randomPosts = await context.Posts
                .Where(p => p.Status == 0)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return randomPosts;
        }

        public async Task<IEnumerable<Post>?> GetsByPropertyAsync(Expression<Func<Post, bool>> predicate, int size = int.MaxValue)
        {
            return await context.Posts.Where(predicate).Take(size).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetsPostByPropertyPagingAsync(Expression<Func<Post, bool>> predicate, int pageNumber, int pageSize)
        {
            var posts = await context.Posts
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return posts;
        }

        public async Task<IEnumerable<Post>> GetAllPostsInGroupsWithValidationAsync(
            string userId, 
            Func<string, Task<GroupDetailsDto?>> getGroupDetails, 
            Func<string, string, Task<bool>> isUserInGroup)
        {
            var posts = await context.Posts
                .Where(p => p.Status == (int)PostStatus.Group) // Only group posts
                .ToListAsync();

            var validPosts = new List<Post>();

            foreach (var post in posts)
            {
                if (string.IsNullOrEmpty(post.GroupId)) continue;

                // Fetch group details using gRPC
                var groupDetails = await getGroupDetails(post.GroupId);

                // Validate group visibility or user's membership
                if (groupDetails != null &&
                    (groupDetails.Visibility == 0 || // Public group
                    await isUserInGroup(post.GroupId, userId))) // User is a group member
                {
                    validPosts.Add(post);
                }
            }

            return validPosts;
        }

        public async Task<IEnumerable<Post>> GetPostsByGroupIdAsync(string groupId)
        {
            return await context.Posts
                .Where(p => p.GroupId == groupId && p.Status == (int)PostStatus.Group) 
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupWithPostCountEntity>> GetGroupsWithPostCountAsync(int minPostCount)
        {
            return await context.Posts
                .Where(p => !string.IsNullOrEmpty(p.GroupId)) 
                .GroupBy(p => p.GroupId)
                .Where(g => g.Count() >= minPostCount)
                .Select(g => new GroupWithPostCountEntity
                {
                    GroupId = g.Key,
                    PostCount = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupPostCount>> GetGroupPostCountsAsync()
        {
            return await context.Posts
                .Where(p => !string.IsNullOrEmpty(p.GroupId)) 
                .GroupBy(p => p.GroupId)
                .Select(g => new GroupPostCount
                {
                    GroupId = g.Key,
                    PostCount = g.Count()
                })
                .ToListAsync();
        }
    }
}
