using Nap.Auth.Business;
using Nap.Auth.Criteria.MenuItem;
using Fanex.Data.Repository;
using Nap.Auth.Model;

namespace Nap.Auth.Service;

public interface MenuItemService
{
    Task<IEnumerable<MenuItem>> GetByUserAndSiteAsync(int userId, int siteId, string env = Constant.Uat);

    Task<IEnumerable<MenuItem>> GetBySiteAsync(int siteId, string env = Constant.Uat, bool? isActive = null);
}

public class MenuItemServiceImp : MenuItemService
{
    private readonly IDynamicRepository repository;

    public MenuItemServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<MenuItem>> GetBySiteAsync(int siteId, string env = "UAT", bool? isActive = null)
    {
        var criteria = new GetMenuItemBySiteCriteria
        {
            SiteId = siteId,
            Environment = env,
            IsActive = isActive
        };

        var menuItems = (await repository.FetchAsync<MenuItem>(criteria).ConfigureAwait(false));

        return menuItems.Select(m => m.ToNonNullMenuItem()).OrderBy(m => m.Name);
    }

    public async Task<IEnumerable<MenuItem>> GetByUserAndSiteAsync(int userId, int siteId, string env = Constant.Uat)
    {
        var criteria = new GetByUserAndSiteIncludeGroupMenuItemCriteria
        {
            UserId = userId,
            SiteId = siteId,
            Environment = env
        };

        var menuItems = (await repository.FetchAsync<MenuItem>(criteria).ConfigureAwait(false));

        return menuItems.Select(m => m.ToNonNullMenuItem()).OrderBy(m => m.Name);
    }
}