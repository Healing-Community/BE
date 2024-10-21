﻿using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class CommentRepository(HFDBPostserviceContext hFDBPostserviceContext) : ICommentRepository
    {
        public async Task Create(Comment entity)
        {
            await hFDBPostserviceContext.Comments.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var reaction = await hFDBPostserviceContext.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
            if (reaction == null) return;
            hFDBPostserviceContext.Comments.Remove(reaction);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Comment> GetByIdAsync(string id)
        {
            return await hFDBPostserviceContext.Comments.FirstAsync(x => x.CommentId == id);
        }

        public async Task<Comment> GetByPropertyAsync(Expression<Func<Comment, bool>> predicate)
        {
            return await hFDBPostserviceContext.Comments.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Comment() { 
                CommentId = Ulid.Empty.ToString() 
            };
        }

        public async Task<IEnumerable<Comment>> GetsAsync()
        {
            return await hFDBPostserviceContext.Comments.ToListAsync();
        }

        public async Task Update(string id, Comment entity)
        {
            var existingComment = await hFDBPostserviceContext.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
            if (existingComment == null) return;
            hFDBPostserviceContext.Entry(existingComment).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingComment).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
