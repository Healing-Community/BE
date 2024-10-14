using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Reports.CreateReport
{
    public record CreateReportCommand(ReportDto ReportDto) : IRequest<BaseResponse<string>>;
}
