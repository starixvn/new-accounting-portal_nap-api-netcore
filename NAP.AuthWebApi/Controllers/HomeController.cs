using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    private readonly SiteService siteService;

    public HomeController(SiteService siteService)
    {
        this.siteService = siteService;
    }

    [Route("")]
    [HttpGet]
    public string Index() => "Swagger link: /swagger/index.html";

    [Route("workingsites")]
    public string WorkingSites()
    {
        var sites = LogSiteData.Sites
            .OrderBy(s => s)
            .Select(s => siteService.GetByIdAsync(s).Result.Name);
        return string.Join(",", sites);
    }
}
