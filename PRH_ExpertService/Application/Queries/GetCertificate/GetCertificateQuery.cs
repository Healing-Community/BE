using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetCertificate
{
    public record GetCertificateQuery(string CertificateId) : IRequest<BaseResponse<Certificate>>;
}
