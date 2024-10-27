using Application.Commons;
using Application.Commons.DTOs;
using Domain.Constants.AMQPMessage;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Report
{
    public record ReportCommand(ReportMessageDto ReportMessageDto) : IRequest<BaseResponse<string>>;

}
