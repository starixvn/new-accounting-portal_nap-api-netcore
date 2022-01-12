using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Business;
using Nap.Auth.Criteria.User;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ReCheckInController : ControllerBase
{
    private readonly UserService userService;
    private readonly SiteService siteService;


    public ReCheckInController(
        UserService userService,
        SiteService siteService)
    {
        this.userService = userService;
        this.siteService = siteService;
    }

    [Route("")]
    [HttpGet()]
    public async Task<int> Get(int onUserId, int siteId)
    {
        var siteStatus = (await siteService.GetById(siteId).ConfigureAwait(false)).IsActive;

        if (!siteStatus)
        {
            return (int)Constant.ReCheckInResult.KickedOut;
        }

        var criteria = new UserReCheckInUserCommand
        {
            OnUserId = onUserId
        };

        await userService.ReUserCheckIn(criteria).ConfigureAwait(false);

        if (criteria.IsKickedOut)
        {
            return (int)Constant.ReCheckInResult.KickedOut;
        }

        return (int)Constant.ReCheckInResult.Ok;
    }
}
