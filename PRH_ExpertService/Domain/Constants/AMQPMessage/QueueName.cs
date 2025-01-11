using System;

namespace Domain.Constants.AMQPMessage;

public enum QueueName
{
    MailQueue,
    PostQueue,
    ReactionQueue,
    CommentQueue,
    PostReportQueue,
    CommentReportQueue, // For admin see moderator 
    AppointmentReportQueue, // For admin see moderator 
    AppointmentModerateQueue, // For admin see moderator

    SyncCommentReportQueue, // to sync with other services
    SyncPostReportQueue, // to sync with other services
    SyncAppointmentReportQueue, // to sync with other services
}
