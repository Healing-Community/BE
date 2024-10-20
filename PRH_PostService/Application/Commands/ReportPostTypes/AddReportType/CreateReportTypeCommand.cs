﻿using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.ReportPostTypes.AddReportType
{
    public record CreateReportTypeCommand(ReportTypeDto ReportTypeDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
