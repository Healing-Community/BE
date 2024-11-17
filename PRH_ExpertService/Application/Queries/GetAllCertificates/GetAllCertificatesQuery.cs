using Application.Commons;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.GetAllCertificates
{
    public record GetAllCertificatesQuery : IRequest<BaseResponse<IEnumerable<Certificate>>>;
}
