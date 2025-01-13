namespace Domain.Constants;

public enum QueueName
{
    MailQueue,
    PostQueue,
    ReactionQueue,
    CommentQueue,
    PostReportQueue,
    CommentReportQueue,
    BanPostQueue,
    BanCommentQueue,
    SyncCommentReportQueue, // to sync with other services
    SyncPostReportQueue, // to sync with other services
    SyncAppointmentReportQueue, // to sync with other services
}