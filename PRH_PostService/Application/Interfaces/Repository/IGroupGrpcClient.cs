using Application.Commons.DTOs;

namespace Application.Interfaces.Repository
{
    public interface IGroupGrpcClient
    {
        Task<bool> CheckGroupExistsAsync(string groupId);
        Task<bool> CheckUserInGroupAsync(string groupId, string userId);
        Task<GroupDetailsDto?> GetGroupDetailsAsync(string groupId);
        Task<bool> CheckUserInGroupOrPublicAsync(string groupId, string userId);
    }
}
