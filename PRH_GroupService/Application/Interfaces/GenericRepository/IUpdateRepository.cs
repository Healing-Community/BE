namespace Application.Interfaces.GenericRepository
{
    public interface IUpdateRepository<in T> where T : class
    {
        Task Update(string id, T entity);
    }
}
