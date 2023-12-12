using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserInfoController(UserService userService) : ControllerBase
{
    [Route("updatePassword")]
    [HttpGet]
    public async Task<bool> UpdatePassword(int siteId, int userId, string password)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        return await userService.ChangePasswordAsync(siteId, userId, password, ip).ConfigureAwait(false);
    }
}
