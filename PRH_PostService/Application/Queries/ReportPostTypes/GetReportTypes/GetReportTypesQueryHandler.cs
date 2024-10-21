using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.ReportPostTypes.GetReportTypes
{
    public class GetReportTypesQueryHandler(IReportTypeRepository reportTypeRepository)
        : IRequestHandler<GetReportTypesQuery, BaseResponse<IEnumerable<ReportType>>>
    {
        public async Task<BaseResponse<IEnumerable<ReportType>>> Handle(GetReportTypesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ReportType>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reportType = await reportTypeRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = reportType;
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
