using System;
using Application.Commons;
using Domain.Entities;
using MassTransit.DependencyInjection.Registration;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Post.Update;

public class UpdatePostReportStatusCommandHandler(IMongoRepository<PostReport> repository): IRequestHandler<UpdatePostReportStatusCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdatePostReportStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var postReport = await repository.GetByPropertyAsync(x => x.PostId == request.PostId);
            if (postReport == null)
            {
                return BaseResponse<string>.NotFound("Bài này đã được duyệt");
            }
            postReport.IsApprove = request.IsApprove;
            await repository.UpdateAsync(postReport.Id, postReport);
            return BaseResponse<string>.SuccessReturn("Duyệt bài thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
