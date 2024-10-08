﻿namespace Domain.Entities
{
    public class NotificationType
    {
        public Guid NotificationTypeId { get; init; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public ICollection<Notification> Notifications { get; set; } = null!;
    }
}
