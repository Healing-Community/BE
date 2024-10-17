using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class CommentRepository(HFDBPostserviceContext hFDBPostserviceContext) : ICommentRepository
    {
        public async Task Create(Comment entity)
        {
            await hFDBPostserviceContext.Comments.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var reaction = await hFDBPostserviceContext.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
            if (reaction == null) return;
            hFDBPostserviceContext.Comments.Remove(reaction);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Comment> GetByIdAsync(Guid id)
        {
            return await hFDBPostserviceContext.Comments.FirstAsync(x => x.CommentId == id);
        }

        public async Task<Comment> GetByPropertyAsync(Expression<Func<Comment, bool>> predicate)
        {
            return await hFDBPostserviceContext.Comments.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Comment();
        }

        public async Task<IEnumerable<Comment>> GetsAsync()
        {
            return await hFDBPostserviceContext.Comments.ToListAsync();
        }

        public async Task Update(Guid id, Comment entity)
        {
            var existingComment = await hFDBPostserviceContext.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
            if (existingComment == null) return;
            hFDBPostserviceContext.Entry(existingComment).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingComment).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
