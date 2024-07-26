using Nap.Auth.Business;
using Nap.Auth.Model;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface MenuItemService
{
    Task<IEnumerable<MenuItem>> GetByUserAndSiteAsync(int userId, int siteId, string env = Constant.Uat);

    Task<IEnumerable<MenuItem>> GetBySiteAsync(int siteId, string env = Constant.Uat, bool? isActive = null);
}

public class MenuItemServiceImp(MemoryCachingStorage cachingService) : MenuItemService
{
    public async Task<IEnumerable<MenuItem>> GetBySiteAsync(int siteId, string env = "UAT", bool? isActive = null)
    {
        var menuItems = cachingService.GetSiteMenuItems(siteId, env, isActive).OrderBy(m => m.Name);

        return await Task.FromResult(menuItems);
    }

    public async Task<IEnumerable<MenuItem>> GetByUserAndSiteAsync(int userId, int siteId, string env = Constant.Uat)
    {
        var menuItems = cachingService.GetUserMenuItems(userId, siteId, env).OrderBy(m => m.Name);

        return await Task.FromResult(menuItems);
    }
}