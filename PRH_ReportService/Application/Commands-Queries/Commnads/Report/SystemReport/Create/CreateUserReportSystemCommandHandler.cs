using Application.Commons;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.Report.SystemReport;

public class CreateUserReportSystemCommandHandler(IMongoRepository<UserReportSystem> repository) : IRequestHandler<CreateUserReportSystemCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateUserReportSystemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userReportSystem = new UserReportSystem
            {
                Id = Ulid.NewUlid().ToString(),
                Content = request.UserReportSystemMessage.Content,
                Email = request.UserReportSystemMessage.Email,
                UserId = request.UserReportSystemMessage.UserId,
                UserName = request.UserReportSystemMessage.UserName,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };
            await repository.Create(userReportSystem);
            return BaseResponse<string>.SuccessReturn("Report has been created successfully");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}