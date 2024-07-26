namespace Nap.Auth.DataCachingPush;

using Fanex.Data.Repository;

public interface DataBatchLoadingService
{
    Task<List<int>> GetSites();

    Task<List<BatchUserFunctionPolicy>> GetUserFunctionPolicies(int siteId = 0);

    Task<List<BatchFunction>> GetFunctions();

    Task<List<BatchGroup>> GetGroups();

    Task<List<BatchUserSiteLevel>> GetUserSiteLevels();

    Task<List<BatchMenuItem>> GetAllMenuItems();

    Task<List<BatchUser>> GetAllUsers();
}

public class DataBatchLoadingServiceImp(IDynamicRepository repository) : DataBatchLoadingService
{
    public async Task<List<int>> GetSites()
    {
        var criteria = new GetAllSiteCriteria { Environment = Constant.Uat, IsActive = null };
        var sites = await repository.FetchAsync<int>(criteria);
        return sites.ToList();
    }

    public async Task<List<BatchUserFunctionPolicy>> GetUserFunctionPolicies(int siteId = 0)
    {
        var criteria = new GetUserFunctionsBySiteCriteria { SiteId = siteId };

        var policies = await repository.FetchAsync<BatchUserFunctionPolicy>(criteria);
        return policies.ToList();
    }

    public async Task<List<BatchFunction>> GetFunctions()
    {
        var criteria = new GetAllFunctionsCriteria();

        var functions = await repository.FetchAsync<BatchFunction>(criteria);

        return functions.ToList();
    }

    public async Task<List<BatchGroup>> GetGroups()
    {
        var criteria = new GetAllGroupsCriteria();
        var groups = await repository.FetchAsync<BatchGroup>(criteria);
        return groups.ToList();
    }

    public async Task<List<BatchUserSiteLevel>> GetUserSiteLevels()
    {
        var criteria = new GetAllUserSiteLevelsCriteria();
        var siteLevels = await repository.FetchAsync<BatchUserSiteLevel>(criteria);
        return siteLevels.ToList();
    }

    public async Task<List<BatchMenuItem>> GetAllMenuItems()
    {
        var criteria = new GetAllMenuItemsCriteria();
        var menuItems = await repository.FetchAsync<BatchMenuItem>(criteria);
        return menuItems.Where(m => m.IsActive).ToList();
    }

    public async Task<List<BatchUser>> GetAllUsers()
    {
        var criteria = new GetAllUsersCriteria();
        var users = await repository.FetchAsync<BatchUser>(criteria);
        return users.ToList();
    }
}