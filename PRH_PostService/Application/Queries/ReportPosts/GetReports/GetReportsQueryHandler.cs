using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.ReportPosts.GetReports
{
    public class GetReportsQueryHandler(IReportRepository reportRepository) 
        : IRequestHandler<GetReportsQuery, BaseResponse<IEnumerable<Report>>>
    {
        public async Task<BaseResponse<IEnumerable<Report>>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Report>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reports = await reportRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
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
