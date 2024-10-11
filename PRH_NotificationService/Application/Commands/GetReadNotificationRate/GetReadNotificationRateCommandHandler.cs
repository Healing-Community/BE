using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.GetReadNotificationRate
{
    public class GetReadNotificationRateCommandHandler : IRequestHandler<GetReadNotificationRateCommand, BaseResponse<double>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetReadNotificationRateCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<double>> Handle(GetReadNotificationRateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<double>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var rate = await _notificationRepository.GetReadNotificationRateAsync();
                response.Data = rate;
                response.Success = true;
                response.Message = "Read notification rate retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve read notification rate.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
