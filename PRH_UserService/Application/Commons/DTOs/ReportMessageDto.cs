using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class ReportMessageDto
    {
        [JsonIgnore] public HttpContext? context { get; set; }
        public Guid TargetUserId { get; set; }
        public Guid ExpertId { get; set; }
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public string? Description { get; set; }

        public required string ReportTypeId { get; set; }
    }
}
