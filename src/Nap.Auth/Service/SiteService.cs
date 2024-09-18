using Nap.Auth.Business;
using Nap.Auth.Model.ValueObject;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface SiteService
{
    Task<IEnumerable<SiteLevel>> GetSiteLevelByUserAsync(int userId, string env = Constant.Uat);
}

public class SiteServiceImp(MemoryCachingStorage cachingService) : SiteService
{
    public async Task<IEnumerable<SiteLevel>> GetSiteLevelByUserAsync(int userId, string env = Constant.Uat)
    {
        var siteLevels = cachingService.GetUserLevels(userId, env)
            .OrderBy(s=>s.Level)
            .ThenBy(s=>s.IsActive)
            .ToList();
        return await Task.FromResult(siteLevels);
    }
}