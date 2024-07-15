using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;
using Newtonsoft.Json;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SsoLoginController(UserService userService) : ControllerBase
{
    [Route("")]
    [HttpGet]
    public async Task<string> GetTime()
    {
        var time = await userService.UserSsoGetTimeAsync().ConfigureAwait(false);
        return JsonConvert.SerializeObject(time);
    }

    [Route("")]
    [HttpPost]
    public async Task<int> Login([FromBody] UserSsoCheckInUserCommand command)
    {
        await userService.UserSsoCheckInAsync(command).ConfigureAwait(false);

        return command.OnUserId > 0 ? command.OnUserId : command.CheckOnUserId;
    }
}