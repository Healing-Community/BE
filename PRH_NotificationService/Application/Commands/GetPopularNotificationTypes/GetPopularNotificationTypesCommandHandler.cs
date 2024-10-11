using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.GetPopularNotificationTypes
{
    public class GetPopularNotificationTypesCommandHandler : IRequestHandler<GetPopularNotificationTypesCommand, BaseResponse<Dictionary<string, int>>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetPopularNotificationTypesCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<Dictionary<string, int>>> Handle(GetPopularNotificationTypesCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Dictionary<string, int>>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var popularTypes = await _notificationRepository.GetPopularNotificationTypesAsync();
                response.Data = popularTypes;
                response.Success = true;
                response.Message = "Popular notification types retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve popular notification types.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
