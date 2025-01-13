namespace Domain.Entities.MBTI21
{
    public class Mbti21
    {
        public required string Id { get; set; }
        public List<MBTICategory>? Categories { get; set; }
    }
}
