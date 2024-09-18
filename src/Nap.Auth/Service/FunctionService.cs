using Nap.Auth.Business;
using Nap.Auth.Model.ValueObject;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface FunctionService
{
    Task<IEnumerable<FunctionPolicy>> GetByUserAndSiteIncludeGroupAsync(int userId, int siteId, string env = Constant.Uat);

    Task<int> GetByKeyAndSiteAsync(string key, int siteId);
}

public class FunctionServiceImp(MemoryCachingStorage cachingService) : FunctionService
{
    public async Task<IEnumerable<FunctionPolicy>> GetByUserAndSiteIncludeGroupAsync(
        int userId,
        int siteId,
        string env = Constant.Uat)
    {
        var policies = cachingService.GetUserFunctions(userId, siteId, env).OrderBy(f => f.Id);
        return await Task.FromResult(policies);
    }

    public async Task<int> GetByKeyAndSiteAsync(string key, int siteId)
    {
        var functionId = cachingService.GetFunctionIdByKey(key, siteId);

        return await Task.FromResult(functionId);
    }
}