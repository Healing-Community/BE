namespace Domain.Entities
{
    public class ReportType
    {
        public Guid ReportTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<Report> Reports { get; set; } 
    }
}
