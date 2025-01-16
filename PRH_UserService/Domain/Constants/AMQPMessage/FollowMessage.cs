namespace Domain.Constants.AMQPMessage
{
    public class FollowMessage
    {
        public string FollowerId { get; set; }
        public string FollowedUserId { get; set; }
        public DateTime FollowDate { get; set; }
    }
}
