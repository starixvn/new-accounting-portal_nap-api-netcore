using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LogoutController(UserService userService) : ControllerBase
{
    [Route("{onUserId:int}")]
    [HttpGet]
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
