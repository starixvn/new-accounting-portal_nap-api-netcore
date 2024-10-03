namespace NAP.AuthWebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Runtime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nap.Auth.DataCachingPull;
using Nap.Auth.DataCachingPush;
using Newtonsoft.Json;

[ApiController]
[Route("cache")]
public class CacheController(CachingReloadService dataCachingService, MemoryCachingStorage storage) : ControllerBase
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
            case JobCommand.UserSiteLevel:
                await dataCachingService.ReloadUserSiteLevels();
                break;
            default:
                break;
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

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Gc()
    {
        const int megabyte = 1024 * 1024;
        var stringBuilder = new StringBuilder();
        var sw = Stopwatch.StartNew();
        sw.Start();

        using var currentProcess = Process.GetCurrentProcess();
        currentProcess.Refresh();
        var memory = currentProcess.WorkingSet64 / megabyte;

        stringBuilder.Append($"Total memory - Before GC.Collect: {memory} Mb");

        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        GC.WaitForPendingFinalizers();
        GC.WaitForFullGCComplete();

        currentProcess.Refresh();
        memory = currentProcess.WorkingSet64 / megabyte;

        var duration = sw.ElapsedMilliseconds;
        stringBuilder.Append($"GC.Collected. Usage memory = {memory}. Duration: {duration} milliseconds");

        return Ok(stringBuilder.ToString());
    }
}