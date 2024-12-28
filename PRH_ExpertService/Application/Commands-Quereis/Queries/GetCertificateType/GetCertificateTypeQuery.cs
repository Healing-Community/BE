using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetCertificateType
{
    public record GetCertificateTypeQuery(string CertificateTypeId) : IRequest<BaseResponse<CertificateType>>;
}