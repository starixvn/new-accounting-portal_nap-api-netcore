using Microsoft.AspNetCore.Mvc;

namespace NAP.AuthWebApi.Controllers;

using Fanex.Caching;
using Microsoft.AspNetCore.Authorization;
using Olympus.Diagnostic;
using ONEbook.UM.Models;
using ONEbook.UM.Services;

[ApiController]
[Route("")]
public class HomeController(ICache cache, IMaintenanceService maintenanceService) : ControllerBase
{
    [Route("")]
    [HttpGet]
    public string Index() => $"Welcome to Nap API. Now is {DateTimeOffset.Now}. Swagger link: /swagger/index.html";

    [Route("Verify")]
    [HttpGet]
    public async Task<string> Verify()
    {
        var cacheOptions = new CacheItemOptions().SetAbsoluteExpiration(DateTimeOffset.Now.AddDays(1));
        await cache.SetAsync("VERIFY-CACHE", DateTimeOffset.Now.ToString(), cacheOptions);
        var (_, value) = await cache.TryGetAsync<string>("VERIFY-CACHE");
        return value;
    }

    [Route("IsUm")]
    [HttpGet]
    public async Task<Dictionary<int, UnderMaintenanceTime>> IsUm()
    {
        return await maintenanceService.GetAllUnderMaintenanceTimeAsync();
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("Gc")]
    public IActionResult Gc()
    {
        var gc = GarbageCollector.CollectExtremely();
        return Ok(gc);
    }
}
