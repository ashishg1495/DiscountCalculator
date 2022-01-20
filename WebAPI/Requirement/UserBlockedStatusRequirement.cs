using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Requirement
{
    public class UserBlockedStatusRequirement : IAuthorizationRequirement
    {
        public bool IsBlocked { get; }
        public UserBlockedStatusRequirement(bool isBlocked)
        {
            IsBlocked = isBlocked;
        }
    }
}


