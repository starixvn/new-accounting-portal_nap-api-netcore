using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    private readonly UserService userService;

    public LogoutController(UserService userService)
    {
        this.userService = userService;
    }

    [Route("{onUserId}")]
    [HttpGet()]
    public async Task<int> Logout(int onUserId)
    {
        var command = new LogOutUserCommand
        {
            OnUserId = onUserId
        };

        await userService.UserLogOutAsync(command).ConfigureAwait(false);

        return 0;
    }
}
