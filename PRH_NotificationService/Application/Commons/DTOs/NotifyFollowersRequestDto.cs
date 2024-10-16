using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.DTOs
{
    public class NotifyFollowersRequestDto
    {
        [JsonIgnore] public HttpContext? Context { get; set; }

        public required Guid UserId { get; set; }
        public required string PostTitle { get; set; }
        public required List<FollowerDto> Followers { get; set; }

    }
    public class FollowerDto
    {
        public Guid UserId { get; set; }
    }
}
