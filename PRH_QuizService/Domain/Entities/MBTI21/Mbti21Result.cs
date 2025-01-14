namespace Domain.Entities.MBTI21
{
    public class Mbti21Result
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public DateTime DateTaken { get; set; }
        public int ExtroversionScore { get; set; }
        public int SensingScore { get; set; }
        public int ThinkingScore { get; set; }
        public int JudgingScore { get; set; }
        public string? ExtroversionDescription { get; set; }
        public string? SensingDescription { get; set; }
        public string? ThinkingDescription { get; set; }
        public string? JudgingDescription { get; set; }
        public string? OverallComment { get; set; }
        public List<string>? Factors { get; set; }
        public List<string>? ShortTermEffects { get; set; }
        public List<string>? LongTermEffects { get; set; }
    }
}
