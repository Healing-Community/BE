using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.ReportPostTypes.AddReportType
{
    public class CreateReportTypeCommandHandler(IReportTypeRepository reportTypeRepository)
        : IRequestHandler<CreateReportTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReportTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var reportType = new ReportType
            {
                ReportTypeId = Ulid.NewUlid().ToString(),
                Name = request.ReportTypeDto.Name,
                Description = request.ReportTypeDto.Description,
            };
            try
            {
                await reportTypeRepository.Create(reportType);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Tạo thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Tạo thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
