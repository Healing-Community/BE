using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class NotifyFollowersRequestDto
    {
        [JsonIgnore] public HttpContext? Context { get; set; }

        public required Guid UserId { get; set; }
        public required string PostTitle { get; set; }
        public required List<FollowerDto> Followers { get; set; } = new List<FollowerDto>();
    }
    public class FollowerDto
    {
        public Guid UserId { get; set; }
    }
}
