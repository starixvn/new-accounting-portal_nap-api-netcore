using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Model;
using Nap.Auth.Model.ValueObject;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService userService;
    private readonly SiteService siteService;
    private readonly MenuItemService menuItemService;
    private readonly FunctionService functionService;

    public UserController(
        UserService userService,
        SiteService siteService,
        MenuItemService menuItemService,
        FunctionService functionService)
    {
        this.userService = userService;
        this.siteService = siteService;
        this.menuItemService = menuItemService;
        this.functionService = functionService;
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<User> Get(int id)
    {
        return await userService.GetById(id).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("")]
    public async Task<User> Get(string username)
    {
        return await userService.GetByUsername(username).ConfigureAwait(false);
    }

    [Route("{userId}/permission")]
    [HttpGet]
    public async Task<int> GetPermission(int userId, int siteId, string functionKey)
    {
        return await userService.GetUserSiteFunctionKeyPolicy(userId, siteId, functionKey).ConfigureAwait(false);
    }

    [Route("{userId}/permissions")]
    [HttpGet]
    public async Task<IEnumerable<FunctionPolicy>> FunctionPolicies(int userId, int siteId, string parentFunctionKey, string environment)
    {
        var functionPolicies = await functionService.GetByUserAndSiteIncludeGroup(userId, siteId, environment).ConfigureAwait(false);

        return string.IsNullOrEmpty(parentFunctionKey)
            ? functionPolicies :
            functionPolicies.Where(f => f.ParentKey == parentFunctionKey);
    }

    [Route("{userId}/menuItem")]
    [HttpGet]
    public async Task<IEnumerable<MenuItem>> MenuItem(int userId, int siteId, string environment)
    {
        return await menuItemService.GetByUserAndSite(userId, siteId, environment).ConfigureAwait(false);
    }

    [Route("{userId}/sitelevel")]
    [HttpGet]
    public async Task<IEnumerable<SiteLevel>> SiteLevel(int userId, string environment)
    {
        return await siteService.GetSiteLevelByUser(userId, environment).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("{userId}/levelonsite/{siteId}")]
    public async Task<int> LevelOnSite(int userId, int siteId)
    {
        return await userService.GetUserSiteLevel(userId, siteId);
    }

    [HttpGet]
    [Route("byfunctionandsite")]
    public async Task<IEnumerable<User>> LevelOnSite(string functionKey, int siteId)
    {
        var functionId = await functionService.GetByKeyAndSite(functionKey, siteId);
        return await userService.GetByFunctionAndSite(functionId, siteId);
    }
}
