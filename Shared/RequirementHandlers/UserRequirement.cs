using Microsoft.AspNetCore.Authorization;

namespace LaciSynchroni.Shared.RequirementHandlers;

public class UserRequirement : IAuthorizationRequirement
{
    public UserRequirement(UserRequirements requirements)
    {
        Requirements = requirements;
    }

    public UserRequirements Requirements { get; }
}
