using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Model;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SiteController(MenuItemService menuItemService) : ControllerBase
{
    [Route("{siteId:int}/menuItem")]
    [HttpGet]
    public async Task<IEnumerable<MenuItem>> GetMenus(int siteId, string environment, bool ? isActive)
    {                
        return await menuItemService.GetBySiteAsync(siteId, environment, isActive);
    }
}