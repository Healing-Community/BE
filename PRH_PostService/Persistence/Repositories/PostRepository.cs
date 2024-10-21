using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;


namespace Persistence.Repositories
{
    public class PostRepository(HFDBPostserviceContext hFDBPostserviceContext) : IPostRepository
    {
        public async Task Create(Post entity)
        {
            await hFDBPostserviceContext.Posts.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var post = await hFDBPostserviceContext.Posts.FirstOrDefaultAsync(x => x.PostId == id);
            if (post == null) return;
            hFDBPostserviceContext.Posts.Remove(post);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Post> GetByIdAsync(string id)
        {
            return await hFDBPostserviceContext.Posts.FirstAsync(x => x.PostId == id);
        }

        public async Task<Post> GetByPropertyAsync(Expression<Func<Post, bool>> predicate)
        {
            var post = await hFDBPostserviceContext.Posts.AsNoTracking().FirstOrDefaultAsync(predicate);
            return post ?? new Post() { PostId = Ulid.Empty.ToString() };
        }

        public async Task<IEnumerable<Post>> GetsAsync()
        {
            return await hFDBPostserviceContext.Posts.ToListAsync();
        }

        public async Task Update(string id, Post entity)
        {
            var existingPost = await hFDBPostserviceContext.Posts.FirstOrDefaultAsync(x => x.PostId == id);
            if (existingPost == null) return;
            hFDBPostserviceContext.Entry(existingPost).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingPost).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
