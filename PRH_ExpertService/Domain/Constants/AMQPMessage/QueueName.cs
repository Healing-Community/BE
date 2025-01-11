using System;

namespace Domain.Constants.AMQPMessage;

public enum QueueName
{
    PostQueue,
    ReactionQueue,
    CommentQueue,
    PostReportQueue,
    CommentReportQueue,
    AppointmentReportQueue,
    AppointmentModerateQueue,
}
