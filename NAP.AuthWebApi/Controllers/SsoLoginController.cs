using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;
using Newtonsoft.Json;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SsoLoginController : ControllerBase
{
    private readonly UserService _userService;

    public SsoLoginController(UserService userService)
    {
        _userService = userService;
    }

    [Route("")]
    [HttpGet()]
    public async Task<string> GetTime()
    {
        var time = await _userService.UserSsoGetTimeAsync().ConfigureAwait(false);
        return JsonConvert.SerializeObject(time);
    }

    [Route("")]
    [HttpPost()]
    public async Task<int> Login([FromBody] UserSSOCheckInUserCommand command)
    {
        await _userService.UserSsoCheckInAsync(command).ConfigureAwait(false);

        return command.OnUserId > 0 ? command.OnUserId : command.CheckOnUserId;
    }
}