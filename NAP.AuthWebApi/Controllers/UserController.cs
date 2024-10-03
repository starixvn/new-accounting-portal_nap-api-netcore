using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Business;
using Nap.Auth.Model;
using Nap.Auth.Model.ValueObject;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserService userService,
        SiteService siteService,
        MenuItemService menuItemService,
        FunctionService functionService)
    : ControllerBase
{
    [Route("{id:int}")]
    [HttpGet]
    public async Task<User> Get(int id)
    {
        return await userService.GetByIdAsync(id).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("")]
    public async Task<User> Get(string username)
    {
        var user = await userService.GetByUsernameAsync(username).ConfigureAwait(false);

        return user;
    }

    [HttpGet]
    [Route("bysite")]
    public async Task<IEnumerable<User>> GetBySite(int siteId, bool? isActive)
    {
        return await userService.GetBySiteAsync(siteId, isActive).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("bysiteandfunction")]
    public IEnumerable<UserFunctionPolicy> GetBySiteAndFunctionKey(string functionKey, int siteId)
    {
        return userService.GetBySiteAndFunctionKey(functionKey, siteId);
    }

    [Route("{userId:int}/permission")]
    [HttpGet]
    public int GetPermission(int userId, int siteId, string functionKey)
    {
        return userService.GetUserSiteFunctionKeyPolicy(userId, siteId, functionKey);
    }

    [Route("{userId:int}/permissions")]
    [HttpGet]
    public IEnumerable<FunctionPolicy> FunctionPolicies(int userId, int siteId, string parentFunctionKey = "", string environment = Constant.Uat)
    {
        var functionPolicies = functionService.GetByUserAndSiteIncludeGroup(userId, siteId, environment);

        return string.IsNullOrEmpty(parentFunctionKey)
            ? functionPolicies :
            functionPolicies.Where(f => f.ParentKey == parentFunctionKey);
    }

    [Route("{userId:int}/menuItem")]
    [HttpGet]
    public IEnumerable<MenuItem> MenuItem(int userId, int siteId, string environment)
    {
        return menuItemService.GetByUserAndSite(userId, siteId, environment);
    }

    [Route("{userId:int}/sitelevel")]
    [HttpGet]
    public IEnumerable<SiteLevel> SiteLevel(int userId, string environment)
    {
        return siteService.GetSiteLevelByUser(userId, environment);
    }

    [HttpGet]
    [Route("{userId:int}/levelonsite/{siteId:int}")]
    public int LevelOnSite(int userId, int siteId)
    {
        return userService.GetUserSiteLevel(userId, siteId);
    }

    [Route("{userId:int}/verify-otp")]
    [HttpGet]
    public async Task<bool> VerifyOtp(int userId, string system, string otp)
    {
        return await userService.VerifyOtpAsync(userId, system, otp);
    }

    [HttpGet]
    [Route("{userId:int}/ignored-otp")]
    public async Task<bool> IgnoredOtp(int userId, string system)
    {
        return await userService.IgnoredOtpAsync(userId, system);
    }
}
