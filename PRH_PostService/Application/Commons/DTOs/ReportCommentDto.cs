using System;

namespace Application.Commons.DTOs;

public class ReportCommentDto
{
    public required string CommentId { get; set; }
    public required ReportTypeEnum ReportTypeEnum { get; set; }
}
