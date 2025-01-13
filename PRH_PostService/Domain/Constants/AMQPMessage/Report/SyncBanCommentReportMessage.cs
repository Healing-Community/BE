using System;

namespace Domain.Constants.AMQPMessage.Report;

public class SyncBanCommentReportMessage
{
    public required string CommentId { get; set; }
    public bool IsApprove { get; set; }
}
