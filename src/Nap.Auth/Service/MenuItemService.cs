using Nap.Auth.Business;
using Nap.Auth.Model;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface MenuItemService
{
    IEnumerable<MenuItem> GetByUserAndSite(int userId, int siteId, string env = Constant.Uat);

    IEnumerable<MenuItem> GetBySite(int siteId, string env = Constant.Uat, bool? isActive = null);
}

public class MenuItemServiceImp(MemoryCachingStorage cachingService) : MenuItemService
{
    public IEnumerable<MenuItem> GetBySite(int siteId, string env = "UAT", bool? isActive = null)
    {
        var menuItems = cachingService.GetSiteMenuItems(siteId, env, isActive).OrderBy(m => m.Name);

        return menuItems;
    }

    public IEnumerable<MenuItem> GetByUserAndSite(int userId, int siteId, string env = Constant.Uat)
    {
        var menuItems = cachingService.GetUserMenuItems(userId, siteId, env).OrderBy(m => m.Name);

        return menuItems;
    }
}