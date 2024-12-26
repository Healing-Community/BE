namespace Application.Interfaces.Repository
{
    public interface IGroupGrpcClient
    {
        Task<bool> CheckGroupExistsAsync(string groupId);
        Task<bool> IsUserInGroupAsync(string userId, string groupId);
    }
}
