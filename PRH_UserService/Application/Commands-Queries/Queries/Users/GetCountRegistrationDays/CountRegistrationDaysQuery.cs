using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetCountRegistrationDays
{
    public record CountRegistrationDaysQuery(string UserId) : IRequest<BaseResponse<UserRegistrationDaysDto>>;

}
