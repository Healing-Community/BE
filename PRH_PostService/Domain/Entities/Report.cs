using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Report
    {
        public Guid ReportId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Guid ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public Post Post { get; set; } = null!;
        public ReportType ReportType { get; set; } = null!;
    }
}
