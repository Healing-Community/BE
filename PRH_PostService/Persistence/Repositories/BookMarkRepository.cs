using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BookMarkRepository(HFDBPostserviceContext context) : IBookMarkRepository
{
    public async Task Create(Bookmark entity)
    {
        await context.Bookmarks.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var bookmark = await context.Bookmarks.FirstOrDefaultAsync(x => x.BookmarkId == id);
        if (bookmark == null) return;
        context.Bookmarks.Remove(bookmark);
        await context.SaveChangesAsync();
    }

    public async Task<Bookmark> GetByIdAsync(string id)
    {
        return await context.Bookmarks.FirstAsync(x => x.BookmarkId == id);
    }

    public async Task<Bookmark?> GetByPropertyAsync(Expression<Func<Bookmark, bool>> predicate)
    {
        return await context.Bookmarks.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Bookmark>> GetsAsync()
    {
        return await context.Bookmarks.ToListAsync();
    }

    public async Task<IEnumerable<Bookmark>> GetsByPropertyAsync(Expression<Func<Bookmark, bool>> predicate , int size = int.MaxValue)
    {
        return await context.Bookmarks.AsNoTracking().Where(predicate).ToListAsync() ?? new List<Bookmark>();
    }

    public async Task Update(string id, Bookmark entity)
    {
        var existingBookmark = await context.Bookmarks.FirstOrDefaultAsync(x => x.BookmarkId == id);
        if (existingBookmark == null) return;
        context.Entry(existingBookmark).CurrentValues.SetValues(entity);
        context.Entry(existingBookmark).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
