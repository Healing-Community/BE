using System.Text.Json.Serialization;

namespace Application.Commons.DTOs
{
    public class Dass21QuizzResultRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public DateTime DateTaken { get; set; }
        public Score? Score { get; set; } 
    }
    public class Score
    {
        public int[]? Stress { get; set; }
        public int[]? Anxiety { get; set; }
        public int[]? Depression { get; set; }
    }
}
