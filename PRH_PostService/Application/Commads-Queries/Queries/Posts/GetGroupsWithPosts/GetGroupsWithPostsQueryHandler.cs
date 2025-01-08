using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;
using NUlid;

namespace Application.Commads_Queries.Queries.Posts.GetGroupsWithPosts
{
    public class GetPublicGroupsWithPostsQueryHandler : IRequestHandler<GetGroupsWithPostsQuery, BaseResponse<IEnumerable<GroupWithPostCountDto>>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IGroupGrpcClient _groupGrpcClient;

        public GetPublicGroupsWithPostsQueryHandler(IPostRepository postRepository, IGroupGrpcClient groupGrpcClient)
        {
            _postRepository = postRepository;
            _groupGrpcClient = groupGrpcClient;
        }

        public async Task<BaseResponse<IEnumerable<GroupWithPostCountDto>>> Handle(GetGroupsWithPostsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<GroupWithPostCountDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy danh sách Group có số bài viết >= 10
                var groupsWithPostCounts = await _postRepository.GetGroupsWithPostCountAsync(10);

                if (!groupsWithPostCounts.Any())
                {
                    response.Success = true;
                    response.Message = "Không có nhóm nào có thể đề cử.";
                    response.Data = new List<GroupWithPostCountDto>();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return response;
                }

                // Lấy thông tin từng Group qua gRPC
                var filteredGroups = new List<GroupWithPostCountDto>();

                foreach (var group in groupsWithPostCounts)
                {
                    var groupInfo = await _groupGrpcClient.GetGroupInfoAsync(group.GroupId);
                    if (groupInfo != null && groupInfo.Visibility == 0) // Chỉ lấy Group Public
                    {
                        filteredGroups.Add(new GroupWithPostCountDto
                        {
                            GroupId = group.GroupId,
                            GroupName = groupInfo.GroupName,
                            PostCount = group.PostCount
                        });
                    }
                }

                if (!filteredGroups.Any())
                {
                    response.Success = true;
                    response.Message = "Không có nhóm nào có thể đề cử.";
                    response.Data = new List<GroupWithPostCountDto>();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return response;
                }

                response.Success = true;
                response.Message = "Lấy danh sách nhóm thành công.";
                response.Data = filteredGroups;
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi khi lấy danh sách nhóm.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
