using Microsoft.AspNetCore.Identity;
using Services.User;

namespace Services;

public abstract class BaseService
{
    protected IdentityUser CurrentUser;

    protected BaseService(IUserService userService)
    {
        CurrentUser = userService.GetCurrentUser().Result;
    }
}