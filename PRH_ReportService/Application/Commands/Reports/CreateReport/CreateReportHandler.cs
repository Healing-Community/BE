using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Reports.CreateReport
{
    public class CreateReportHandler(IReportRepository reportRepository,IMapper mapper) : IRequestHandler<CreateReportCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };


            try
            {
                var report = mapper.Map<Report>(request.ReportDto);
                report.ReportId = NewId.NextSequentialGuid();

                await reportRepository.Create(report);
                response.Message = "Report created!";
                response.Data = "report created!";
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                response.Success = false;
                response.Data = null;
                response.Message = "An error occurred";
                return response;
            }
        }
    }
}
