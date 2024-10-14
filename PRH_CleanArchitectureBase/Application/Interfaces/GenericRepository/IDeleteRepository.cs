namespace Application.Interfaces.GenericRepository;

public interface IDeleteRepository
{
    Task DeleteAsync(Guid id);
}