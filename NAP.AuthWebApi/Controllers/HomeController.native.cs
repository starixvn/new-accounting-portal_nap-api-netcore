using Microsoft.AspNetCore.Mvc;

namespace NAP.AuthWebApi.Controllers;

using System.Diagnostics;
using System.Runtime;
using System.Text;
using Fanex.Caching;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("")]
public class HomeController(ICache cache) : ControllerBase
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

    [AllowAnonymous]
    [HttpGet]
    [Route("Gc")]
    public IActionResult Gc()
    {
        const int megabyte = 1024 * 1024;
        var stringBuilder = new StringBuilder();
        var sw = Stopwatch.StartNew();
        sw.Start();

        using var currentProcess = Process.GetCurrentProcess();
        currentProcess.Refresh();
        var memory = currentProcess.WorkingSet64 / megabyte;

        stringBuilder.Append($"Before GC.Collect: {memory} Mb.\r\n");

        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        GC.WaitForPendingFinalizers();
        GC.WaitForFullGCComplete();

        currentProcess.Refresh();
        memory = currentProcess.WorkingSet64 / megabyte;

        var duration = sw.ElapsedMilliseconds;
        stringBuilder.Append($"After GC.Collect. Usage memory = {memory}.\r\n");
        stringBuilder.Append($"Duration: {duration} milliseconds.\r\n");

        return Ok(stringBuilder.ToString());
    }
}
