using Nap.Auth.Business;
using Nap.Auth.Model.ValueObject;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface SiteService
{
    IEnumerable<SiteLevel> GetSiteLevelByUser(int userId, string env = Constant.Uat);
}

public class SiteServiceImp(MemoryCachingStorage cachingService) : SiteService
{
    public IEnumerable<SiteLevel> GetSiteLevelByUser(int userId, string env = Constant.Uat)
    {
        var siteLevels = cachingService.GetUserLevels(userId, env)
            .OrderBy(s=>s.Level)
            .ThenBy(s=>s.IsActive)
            .ToList();

        return siteLevels;
    }
}