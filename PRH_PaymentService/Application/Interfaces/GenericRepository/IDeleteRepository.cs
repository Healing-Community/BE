namespace Application.Interfaces.GenericRepository;

public interface IDeleteRepository
{
    Task DeleteAsync(string id);
}