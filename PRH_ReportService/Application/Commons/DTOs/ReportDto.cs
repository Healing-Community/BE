using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class ReportDto
    {
        public required string UserId { get; set; }
        public string? TargetUserId { get; set; }
        public string? ExpertId { get; set; }
        public string? PostId { get; set; }
        public string? CommentId { get; set; }
        public required string ReportTypeId { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
