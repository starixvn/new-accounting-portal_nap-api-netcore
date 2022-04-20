using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Model;
using Nap.Auth.Service;
using NAP.AuthWebApi.HostedServices;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SiteController : ControllerBase
{
    private readonly MenuItemService menuItemService;

    public SiteController(MenuItemService menuItemService)
    {
        this.menuItemService = menuItemService;
    }

    [Route("{siteId}/menuItem")]
    [HttpGet()]
    public async Task<IEnumerable<MenuItem>> GetMenus(int siteId, string environment, bool ? isActive)
    {                
        return await menuItemService.GetBySite(siteId, environment, isActive);
    }
}