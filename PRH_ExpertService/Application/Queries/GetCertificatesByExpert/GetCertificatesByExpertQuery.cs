using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetCertificatesByExpert
{
    public record GetCertificatesByExpertQuery(string ExpertProfileId) : IRequest<BaseResponse<IEnumerable<Certificate>>>;
}
