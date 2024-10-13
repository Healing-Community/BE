﻿using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.UpdateNotificationPreference
{
    public class UpsertNotificationPreferenceCommandHandler : IRequestHandler<UpsertNotificationPreferenceCommand, BaseResponse<string>>
    {
        private readonly IUserNotificationPreferenceRepository _userNotificationPreferenceRepository;
        private readonly INotificationTypeRepository _notificationTypeRepository;

        public UpsertNotificationPreferenceCommandHandler(IUserNotificationPreferenceRepository userNotificationPreferenceRepository, 
            INotificationTypeRepository notificationTypeRepository)
        {
            _userNotificationPreferenceRepository = userNotificationPreferenceRepository;
            _notificationTypeRepository = notificationTypeRepository;
        }

        public async Task<BaseResponse<string>> Handle(UpsertNotificationPreferenceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var notificationType = await _notificationTypeRepository.GetByNameAsync(request.NotificationType.ToString());
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = "Invalid notification type.";
                    return response;
                }

                var preference = await _userNotificationPreferenceRepository.GetByUserIdAndNotificationTypeIdAsync(request.UserId, notificationType.NotificationTypeId);

                if (preference == null)
                {
                    preference = new UserNotificationPreference
                    {
                        UserId = request.UserId,
                        NotificationTypeId = notificationType.NotificationTypeId,
                        IsSubscribed = request.IsSubscribed
                    };
                    await _userNotificationPreferenceRepository.Create(preference);
                }
                else
                {
                    preference.IsSubscribed = request.IsSubscribed;
                    await _userNotificationPreferenceRepository.Update(preference);
                }

                response.Success = true;
                response.Message = "Notification preference updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update notification preference.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}