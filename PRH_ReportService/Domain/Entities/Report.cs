﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Report
    {
        public Guid ReportId { get; set; }
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; }
        public Guid ExpertId { get; set; }
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public required string ReportTypeId { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }

        public ReportType? ReportType { get; set; }
    }
}
