using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ReportType
{
    public required string ReportTypeId { get; set; }

    [MaxLength(100)] public required string Name { get; set; }

    [MaxLength(200)] public required string Description { get; set; }

    public ICollection<Report>? Reports { get; set; }
}