using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.ReportPostTypes.AddReportType
{
    public class CreateReportTypeCommandHandler(IReportTypeRepository reportTypeRepository)
        : IRequestHandler<CreateReportTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReportTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var reportType = new ReportType
            {
                ReportTypeId = NewId.NextSequentialGuid(),
                Name = request.ReportTypeDto.Name,
                Description = request.ReportTypeDto.Description,
            };
            try
            {
                await reportTypeRepository.Create(reportType);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Report type created successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to create report type";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
