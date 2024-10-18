using Application.Commons;
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

namespace Application.Queries.ReportPosts.GetReports
{
    public class GetReportsQueryHandler(IReportRepository reportRepository) 
        : IRequestHandler<GetReportsQuery, BaseResponse<IEnumerable<Report>>>
    {
        public async Task<BaseResponse<IEnumerable<Report>>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Report>>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reports = await reportRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Report retrieved successfully";
                response.Success = true;
                response.Data = reports;
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
