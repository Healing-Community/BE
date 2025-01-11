namespace Domain.Constants;

public enum QueueName
{
    PostQueue,
    ReactionQueue,
    CommentQueue,
    PostReportQueue,
    CommentReportQueue,
    AppointmentReportQueue,
    BanPostQueue,
    BanCommentQueue,
    AppointmentModerateQueue,
    SyncCommentReportQueue, // to sync with other services
    SyncPostReportQueue, // to sync with other services
    SyncAppointmentReportQueue, // to sync with other services
}