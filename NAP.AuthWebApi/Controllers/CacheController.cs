namespace NAP.AuthWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Nap.Auth.DataCachingPush;

[ApiController]
[Route("cache")]
public class CacheController(DataCachingService dataCachingService) : ControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Action(string command, int siteId)
    {
        var jobCommand = new JobCommand(command, siteId);
        var cacheVersion = await SolveCommand(jobCommand);

        return Content(cacheVersion);
    }

    [HttpGet]
    [Route("reload")]
    public async Task<IActionResult> Reload()
    {
        await dataCachingService.ReloadAll();
        var cacheVersion = dataCachingService.CacheVersion;

        return Content(cacheVersion);
    }

    private async Task<string> SolveCommand(JobCommand command)
    {
        switch (command.Command)
        {
            case JobCommand.Site:
                await dataCachingService.ReloadSites();
                break;
            case JobCommand.Function:
                await dataCachingService.ReloadFunctions();
                break;
            case JobCommand.Group:
                await dataCachingService.ReloadGroups();
                break;
            case JobCommand.Menu:
                await dataCachingService.ReloadSiteMenu();
                break;
            case JobCommand.UserFunctionPolicy:
                await dataCachingService.ReloadUserFunctionPolicy(command.SiteId);
                break;
            case JobCommand.User:
                await dataCachingService.ReloadUsers();
                break;
            case JobCommand.UserOtp:
                await dataCachingService.ReloadUserOtp();
                break;
            case JobCommand.UserSiteLevel:
                await dataCachingService.ReloadUserSiteLevels();
                break;
        }

        return dataCachingService.CacheVersion;
    }
}