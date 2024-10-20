using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class CategoryRepository(HFDBPostserviceContext hFDBPostserviceContext) : ICategoryRepository
    {
        public async Task Create(Category entity)
        {
            await hFDBPostserviceContext.Categories.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await hFDBPostserviceContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null) return;
            hFDBPostserviceContext.Categories.Remove(category);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await hFDBPostserviceContext.Categories.FirstAsync(x => x.CategoryId == id);
        }

        public async Task<Category> GetByPropertyAsync(Expression<Func<Category, bool>> predicate)
        {
            return await hFDBPostserviceContext.Categories.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Category();
        }

        public async Task<IEnumerable<Category>> GetsAsync()
        {
            return await hFDBPostserviceContext.Categories.ToListAsync();
        }

        public async Task Update(Guid id, Category entity)
        {
            var existingCategory = await hFDBPostserviceContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingCategory == null) return;
            hFDBPostserviceContext.Entry(existingCategory).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingCategory).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
