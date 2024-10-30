namespace Nap.Auth.DataCachingPush;

using Fanex.Caching;

public interface CachingRefresherService
{
    Task RefreshSites();

    Task RefreshAll();

    Task RefreshFunctions();

    Task RefreshGroups();

    Task RefreshUserSiteLevels();

    Task RefreshUserFunctionPolicy(int siteId = 0);

    Task RefreshSiteMenu();

    Task RefreshUsers();

    string CacheVersion { get; }
}


public class CachingRefresherServiceImp(DataBatchLoadingService repository, ICache cache) : CachingRefresherService
{
    private const string CacheVersionKey = "NAP_API_CACHE_VERSION";

    private CacheItemOptions CacheOptions => new() { AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(7) };

    internal static class NapApiCacheKey
    {
        internal const string CacheVersion = "NAP_API_CACHE_VERSION";

        internal const string Functions = "NAP_API_ALL_FUNCTIONS";

        internal const string Groups = "NAP_API_ALL_GROUPS";

        internal const string UserSiteLevels = "NAP_API_ALL_USER_SITE_LEVELS";

        internal const string UserFunctionPolicies = "NAP_API_ALL_USER_FUNCTION_POLICIES_";

        internal const string SiteMenuItems = "NAP_API_ALL_SITE_MENU_ITEMS";

        internal const string UserOTP = "NAP_API_ALL_USER_OTP";

        internal const string Users = "NAP_API_ALL_USERS";

        internal const string Sites = "NAP_API_ALL_SITES";
    }

    public string CacheVersion
    {
        get
        {
            cache.TryGet<string>(NapApiCacheKey.CacheVersion, out var cacheVersion);
            return cacheVersion;
        }
    }

    public async Task RefreshAll()
    {
        await RefreshSites();
        await RefreshFunctions();
        await RefreshGroups();
        await RefreshSiteMenu();
        await RefreshUserSiteLevels();
        await RefreshUserFunctionPolicy();
        await RefreshUsers();
    }

    public async Task RefreshSites()
    {
        var sites = await repository.GetSites();

        await cache.SetAsync(NapApiCacheKey.Sites, sites, CacheOptions);
        await SaveCacheVersion();
    }

    public async Task RefreshFunctions()
    {
        var functionList = (await repository.GetFunctions()).Select(f => f.ToNonNullFunction())
            .ToList();
        await cache.SetAsync(NapApiCacheKey.Functions, functionList, CacheOptions);
        await SaveCacheVersion();
    }

    public async Task RefreshGroups()
    {
        var groupList = (await repository.GetGroups()).Select(u => u.ToNonNullGroup())
            .ToList();

        await cache.SetAsync(NapApiCacheKey.Groups, groupList, CacheOptions);
        await SaveCacheVersion();
    }

    public async Task RefreshUserSiteLevels()
    {
        var siteLevelList = await repository.GetUserSiteLevels();

        await cache.SetAsync(NapApiCacheKey.UserSiteLevels, siteLevelList, CacheOptions);
        await SaveCacheVersion();
    }

    public async Task RefreshUserFunctionPolicy(int siteId = 0)
    {
        var policies = await repository.GetUserFunctionPolicies(siteId);
        var policiesBySite = policies.GroupBy(p => p.SiteId)
            .ToDictionary(p => p.Key, p => p.ToList());
        foreach (var id in policiesBySite.Keys)
        {
            await cache.SetAsync(NapApiCacheKey.UserFunctionPolicies + id, policiesBySite[id], CacheOptions);
        }
        await SaveCacheVersion();
    }

    public async Task RefreshSiteMenu()
    {
        var menuItems = (await repository.GetAllMenuItems())
            .Select(m => m.ToNonNullMenuItem())
            .ToList();
        await cache.SetAsync(NapApiCacheKey.SiteMenuItems, menuItems, CacheOptions);
        await SaveCacheVersion();
    }

    public async Task RefreshUsers()
    {
        var users = (await repository.GetAllUsers())
            .Select(u => u.ToNonNullUser())
            .ToList();
        await cache.SetAsync(NapApiCacheKey.Users, users, CacheOptions);
        await SaveCacheVersion();
    }

    private async Task SaveCacheVersion()
    {
        var version = $"{Guid.NewGuid()}-{DateTimeOffset.Now:yyyy-MM-dd-HH:mm:ss} - GMT+{DateTimeOffset.Now.Offset.Hours}";
        await cache.SetAsync(CacheVersionKey, version, CacheOptions);
    }
}