namespace Application.Commons.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
    }
}
