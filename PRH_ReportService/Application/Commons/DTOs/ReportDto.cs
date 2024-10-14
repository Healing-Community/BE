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
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; }
        public Guid ExpertId { get; set; }
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public required string ReportTypeId { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
