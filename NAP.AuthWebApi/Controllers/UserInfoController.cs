using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserInfoController : ControllerBase
{
    private readonly UserService userService;

    public UserInfoController(UserService userService)
    {
        this.userService = userService;
    }

    [Route("updatePassword")]
    [HttpGet]
    public async Task<bool> UpdatePassword(int siteId, int userId, string password)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        return await userService.ChangePasswordAsync(siteId, userId, password, ip).ConfigureAwait(false);
    }
}
