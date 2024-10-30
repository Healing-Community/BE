﻿namespace Domain.Constants.AMQPMessage.Post
{
    public class PostingRequest
    {
        public required string PostId { get; set; }
        public Guid UserId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }

    }
}