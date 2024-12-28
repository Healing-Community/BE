using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAllCertificateTypes
{
    public record GetAllCertificateTypesQuery : IRequest<BaseResponse<IEnumerable<CertificateType>>>;
}