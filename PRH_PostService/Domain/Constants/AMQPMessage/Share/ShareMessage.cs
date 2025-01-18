namespace Domain.Constants.AMQPMessage.Share
{
    public class ShareMessage
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Platform { get; set; }
        public string Description { get; set; }
        public DateTime ShareDate { get; set; }
    }
}
