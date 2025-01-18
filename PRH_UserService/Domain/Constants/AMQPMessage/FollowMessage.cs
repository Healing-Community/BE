namespace Domain.Constants.AMQPMessage
{
    public class FollowMessage
    {
        public string FollowerId { get; set; }
        public string FollowedUserId { get; set; }
        public string FollowedUserName { get; set; }
        public string FollowingUserName { get; set; }
        public DateTime FollowDate { get; set; }
    }
}
