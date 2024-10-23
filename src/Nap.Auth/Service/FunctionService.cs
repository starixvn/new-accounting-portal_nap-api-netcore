using Nap.Auth.Business;
using Nap.Auth.Model.ValueObject;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface FunctionService
{
    IEnumerable<FunctionPolicy> GetByUserAndSiteIncludeGroup(int userId, int siteId, string env = Constant.Uat);

    int GetByKeyAndSite(string key, int siteId);
}

public class FunctionServiceImp(MemoryCachingStorage cachingService) : FunctionService
{
    public IEnumerable<FunctionPolicy> GetByUserAndSiteIncludeGroup(
        int userId,
        int siteId,
        string env = Constant.Uat)
    {
        var policies = cachingService.GetUserFunctions(userId, siteId, env).OrderBy(f => f.Id);
        
        return policies;
    }

    public int GetByKeyAndSite(string key, int siteId)
    {
        var functionId = cachingService.GetFunctionIdByKey(key, siteId);

        return functionId;
    }
}