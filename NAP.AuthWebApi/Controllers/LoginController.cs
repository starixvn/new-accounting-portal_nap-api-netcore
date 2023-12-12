using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(UserService userService) : ControllerBase
{
    [Route("")]
    [HttpPost]
    public async Task<int> Login([FromBody] UserCheckInUserCommand command)
    {
        await userService.UserCheckInAsync(command).ConfigureAwait(false);

        return command.OnUserId;
    }
}
