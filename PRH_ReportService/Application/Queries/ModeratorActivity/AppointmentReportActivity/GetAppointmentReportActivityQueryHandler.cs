using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Queries.ModeratorActivity.AppointmentReportActivity;

public class GetAppointmentReportActivityQueryHandler(IMongoRepository<ModerateApointmentReportActivity> repository) : IRequestHandler<GetAppointmentReportActivityQuery, BaseResponse<IEnumerable<ModerateApointmentReportActivity>>>
{
    public async Task<BaseResponse<IEnumerable<ModerateApointmentReportActivity>>> Handle(GetAppointmentReportActivityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetsAsync();
            return BaseResponse<IEnumerable<ModerateApointmentReportActivity>>.SuccessReturn(result);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<ModerateApointmentReportActivity>>.InternalServerError(e.Message);
            throw;
        }

    }
}