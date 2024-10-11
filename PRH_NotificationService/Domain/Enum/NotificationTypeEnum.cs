using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum NotificationTypeEnum
    {
        Comment = 1,
        Like = 2,
        Follower = 3,
        Message = 4,
        NewPostByFollowedUser = 5,
        Report = 6,
        StoryApproved = 7,
        StoryRejected = 8,
        Mention = 9,
        ScheduledEvent = 10,
    }
}
