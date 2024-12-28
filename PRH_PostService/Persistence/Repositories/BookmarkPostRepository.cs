using System;
using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BookmarkPostRepository(HFDBPostserviceContext context) : IBookmarkPostRepository
{
    public async Task Create(BookmarkPost entity)
    {
        await context.BookmarkPosts.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var bookmarkPost = await context.BookmarkPosts.FindAsync(id);
        if (bookmarkPost == null) return;
        context.BookmarkPosts.Remove(bookmarkPost);
        await context.SaveChangesAsync();
    }

    public async Task<BookmarkPost> GetByIdAsync(string id)
    {
        return await context.BookmarkPosts.FirstAsync(x => x.BookmarkPostId == id);
    }

    public async Task<BookmarkPost?> GetByPropertyAsync(Expression<Func<BookmarkPost, bool>> predicate)
    {
        return await context.BookmarkPosts.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<BookmarkPost>> GetsAsync()
    {
        return await context.BookmarkPosts.ToListAsync();
    }

    public async Task<IEnumerable<BookmarkPost>?> GetsByPropertyAsync(Expression<Func<BookmarkPost, bool>> predicate , int size = int.MaxValue)
    {
        return await context.BookmarkPosts.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task Update(string id, BookmarkPost entity)
    {
        var existingBookmarkPost = await context.BookmarkPosts.FindAsync(id);
        if (existingBookmarkPost == null) return;
        context.Entry(existingBookmarkPost).CurrentValues.SetValues(entity);
        context.Entry(existingBookmarkPost).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
