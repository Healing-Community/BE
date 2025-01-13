using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.ModeratorActivity.AppointmentReportActivity;

public class CreateAppointmentReportActivityCommandHandler(IMongoRepository<ModerateApointmentReportActivity> repository) : IRequestHandler<CreateAppointmentReportActivityCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateAppointmentReportActivityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new ModerateApointmentReportActivity
            {
                AppointmentId = request.Message.AppointmentId,
                Id = Ulid.NewUlid().ToString(),
                ModeratorEmail = request.Message.ModeratorEmail,
                ModeratorId = request.Message.ModeratorId,
                ModeratorName = request.Message.ModeratorName,
                AppoinmtentDate = request.Message.AppoinmtentDate,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                EndTime = request.Message.EndTime,
                ExpertEmail = request.Message.ExpertEmail,
                ExpertName = request.Message.ExpertName,
                IsApprove = request.Message.IsApprove,
                StartTime = request.Message.StartTime,
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UserEmail = request.Message.UserEmail,
                UserName = request.Message.UserName
            };

            await repository.Create(entity);

            return BaseResponse<string>.SuccessReturn();
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}
