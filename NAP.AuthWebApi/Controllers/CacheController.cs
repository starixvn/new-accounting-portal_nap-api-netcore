namespace NAP.AuthWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Nap.Auth.DataCachingPull;
using Nap.Auth.DataCachingPush;
using Newtonsoft.Json;

[ApiController]
[Route("cache")]
public class CacheController(CachingRefresherService dataCachingService, MemoryCachingStorage storage, Fanex.Logging.ILogger logger) : ControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Action(string command, int siteId)
    {
        await logger.InfoAsync($"Command: {command}. SiteId: {siteId} at {DateTimeOffset.Now}");
        var jobCommand = new JobCommand(command, siteId);
        var cacheVersion = await SolveCommand(jobCommand);

        return Content(cacheVersion);
    }

    [HttpGet]
    [Route("reload")]
    public async Task<IActionResult> Reload()
    {
        var oldCacheVersion = dataCachingService.CacheVersion;
        await dataCachingService.RefreshAll();
        var newCacheVersion = dataCachingService.CacheVersion;

        await logger.InfoAsync($"Reload Old: {oldCacheVersion} - New: {newCacheVersion} at {DateTimeOffset.Now}");
        if (string.IsNullOrEmpty(newCacheVersion))
        {
            await logger.ErrorAsync("There is a cache issue in NAP API");
        }

        return Content($"Old: {oldCacheVersion} - New: {newCacheVersion}");
    }
    
    private async Task<string> SolveCommand(JobCommand command)
    {
        switch (command.Command)
        {
            case JobCommand.Site:
                await dataCachingService.RefreshSites();
                break;
            case JobCommand.Function:
                await dataCachingService.RefreshFunctions();
                break;
            case JobCommand.Group:
                await dataCachingService.RefreshGroups();
                break;
            case JobCommand.Menu:
                await dataCachingService.RefreshSiteMenu();
                break;
            case JobCommand.UserFunctionPolicy:
                await dataCachingService.RefreshUserFunctionPolicy(command.SiteId);
                break;
            case JobCommand.User:
                await dataCachingService.RefreshUsers();
                break;
            case JobCommand.UserSiteLevel:
                await dataCachingService.RefreshUserSiteLevels();
                break;
            default:
                await Task.Delay(10);
                break;
        }

        if (string.IsNullOrEmpty(dataCachingService.CacheVersion))
        {
            await logger.ErrorAsync("There is a cache issue in NAP API");
        }

        return dataCachingService.CacheVersion;
    }

    #region Debug

    [HttpGet]
    [Route("SiteUserFunctionPoliciesCache")]
    public IActionResult SiteUserFunctionPoliciesCache()
    {
        var json = JsonConvert.SerializeObject(storage.SiteUserFunctionPoliciesCache);
        return Content(json);
    }

    [HttpGet]
    [Route("SiteUserMenuItemsCache")]
    public IActionResult SiteUserMenuItemsCache()
    {
        var json = JsonConvert.SerializeObject(storage.SiteUserMenuItemsCache);
        return Content(json);
    }

    [HttpGet]
    [Route("FunctionsCache")]
    public IActionResult FunctionsCache()
    {
        var json = JsonConvert.SerializeObject(storage.FunctionsCache);
        return Content(json);
    }

    [HttpGet]
    [Route("GroupsCache")]
    public IActionResult GroupsCache()
    {
        var json = JsonConvert.SerializeObject(storage.GroupsCache);
        return Content(json);
    }

    [HttpGet]
    [Route("UserSiteLevelsByUserCache")]
    public IActionResult UserSiteLevelsByUserCache()
    {
        var json = JsonConvert.SerializeObject(storage.UserSiteLevelsByUserCache);
        return Content(json);
    }

    [HttpGet]
    [Route("UserSiteLevelsBySiteCache")]
    public IActionResult UserSiteLevelsBySiteCache()
    {
        var json = JsonConvert.SerializeObject(storage.UserSiteLevelsBySiteCache);
        return Content(json);
    }

    [HttpGet]
    [Route("SiteMenuItemsCache")]
    public IActionResult SiteMenuItemsCache()
    {
        var json = JsonConvert.SerializeObject(storage.SiteMenuItemsCache);
        return Content(json);
    }

    [HttpGet]
    [Route("UserOtpCache")]
    public IActionResult UserOtpCache()
    {
        var json = JsonConvert.SerializeObject(storage.UserOtpCache);
        return Content(json);
    }

    [HttpGet]
    [Route("UsersCache")]
    public IActionResult UsersCache()
    {
        var json = JsonConvert.SerializeObject(storage.UsersCache);
        return Content(json);
    }

    #endregion
}