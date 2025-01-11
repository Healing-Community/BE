namespace Application.Interfaces.GenericRepository;

public interface IUpdateRepository<in T> where T : class
{
    Task UpdateAsync(string id, T entity);
}