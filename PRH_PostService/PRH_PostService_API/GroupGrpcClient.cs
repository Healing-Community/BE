using Grpc.Net.Client;
using Application.Interfaces.Repository;
using GroupServiceGrpc;
using Application.Commons.DTOs;

namespace PRH_PostService_API
{
    public class GroupGrpcClient : IGroupGrpcClient
    {
        private readonly GroupService.GroupServiceClient _client;

        public GroupGrpcClient(IConfiguration configuration)
        {
            // Đọc URL của Group Service từ cấu hình
            var groupServiceUrl = configuration["GrpcSettings:GroupServiceUrl"];

            // Cấu hình HttpHandler để bỏ qua kiểm tra chứng chỉ trong môi trường phát triển
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            // Khởi tạo GrpcChannel với HttpHandler cấu hình sẵn
            var channel = GrpcChannel.ForAddress(groupServiceUrl, new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });

            // Tạo một instance của GroupServiceClient
            _client = new GroupService.GroupServiceClient(channel);
        }

        /// <summary>
        /// Kiểm tra xem một Group có tồn tại hay không bằng cách gọi gRPC tới Group Service.
        /// </summary>
        /// <param name="groupId">ID của Group cần kiểm tra</param>
        /// <returns>True nếu Group tồn tại, ngược lại là False</returns>
        public async Task<bool> CheckGroupExistsAsync(string groupId)
        {
            // Tạo request với GroupId
            var request = new CheckGroupRequest { GroupId = groupId };

            try
            {
                // Gọi gRPC tới Group Service
                var response = await _client.CheckGroupExistsAsync(request);
                return response.Exists; // Trả về kết quả từ response
            }
            catch (Grpc.Core.RpcException ex)
            {
                // Xử lý lỗi gRPC (nếu cần)
                Console.WriteLine($"Lỗi khi gọi gRPC: {ex.Status.Detail}");
                return false; // Trả về false nếu có lỗi xảy ra
            }
        }
        public async Task<bool> CheckUserInGroupAsync(string groupId, string userId)
        {
            var request = new CheckUserInGroupRequest
            {
                GroupId = groupId,
                UserId = userId
            };

            var response = await _client.CheckUserInGroupAsync(request);
            return response.IsMember;
        }
        public async Task<bool> CheckUserInGroupOrPublicAsync(string groupId, string userId)
        {
            var request = new CheckUserInGroupRequest
            {
                GroupId = groupId,
                UserId = userId
            };

            var response = await _client.CheckUserInGroupOrPublicAsync(request);
            return response.IsMember;
        }
        public async Task<GroupDetailsDto?> GetGroupDetailsAsync(string groupId)
        {
            var request = new GetGroupDetailsRequest { GroupId = groupId };

            var response = await _client.GetGroupDetailsAsync(request);

            return response != null
                ? new GroupDetailsDto
                {
                    GroupId = response.GroupId,
                    Visibility = response.Visibility
                }
                : null;
        }
        public async Task<string?> GetUserRoleInGroupAsync(string groupId, string userId)
        {
            var request = new GetUserRoleInGroupRequest
            {
                GroupId = groupId,
                UserId = userId
            };

            try
            {
                var response = await _client.GetUserRoleInGroupAsync(request);
                return response.Role; // Trả về vai trò của user
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"Lỗi khi gọi gRPC: {ex.Status.Detail}");
                return null; // Trả về null nếu lỗi
            }
        }
        public async Task<GroupInfoDto?> GetGroupInfoAsync(string groupId)
        {
            var request = new GetGroupInfoRequest { GroupId = groupId };

            try
            {
                var response = await _client.GetGroupInfoAsync(request);
                return response != null
                    ? new GroupInfoDto
                    {
                        GroupId = response.GroupId,
                        GroupName = response.GroupName,
                        Visibility = response.Visibility
                    }
                    : null;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"Lỗi khi gọi gRPC: {ex.Status.Detail}");
                return null; // Trả về null nếu có lỗi xảy ra
            }
        }
    }
}
