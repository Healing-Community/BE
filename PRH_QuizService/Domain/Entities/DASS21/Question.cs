namespace Domain.Entities.DASS21
{
    public class Question
    {
        public string? QuestionText { get; set; }
        public string? Type { get; set; }
        public List<string>? Options { get; set; }
    }
}