using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.SystemReport;

public record GetsUserSystemReportQuery : IRequest<BaseResponse<IEnumerable<UserReportSystem>>>;