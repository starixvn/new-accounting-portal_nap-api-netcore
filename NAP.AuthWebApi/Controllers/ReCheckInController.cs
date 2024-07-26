using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Business;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ReCheckInController(UserService userService) : ControllerBase
{
    [Route("")]
    [HttpGet]
    public async Task<int> Get(int onUserId, int siteId)
    {
        var criteria = new UserReCheckInUserCommand
        {
            OnUserId = onUserId
        };

        await userService.ReUserCheckInAsync(criteria).ConfigureAwait(false);

        if (criteria.IsKickedOut)
        {
            return (int)Constant.ReCheckInResult.KickedOut;
        }

        return (int)Constant.ReCheckInResult.Ok;
    }
}
