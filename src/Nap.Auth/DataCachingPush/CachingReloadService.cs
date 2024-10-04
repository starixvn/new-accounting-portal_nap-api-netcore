namespace Nap.Auth.DataCachingPush;

using Fanex.Caching;

public interface CachingReloadService
{
    Task ReloadSites();

    Task ReloadAll();

    Task ReloadFunctions();

    Task ReloadGroups();

    Task ReloadUserSiteLevels();

    Task ReloadUserFunctionPolicy(int siteId = 0);

    Task ReloadSiteMenu();

    Task ReloadUsers();

    string CacheVersion { get; }
}


public class CachingReloadServiceImp(DataBatchLoadingService repository, ICache cache) : CachingReloadService
{
    private const string CacheVersionKey = "NAP_API_CACHE_VERSION";

    private readonly CacheItemOptions cacheOptions = new() { AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(7) };

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
            cache.TryGet<string>(NapApiCacheKey.CacheVersion, out var  cacheVersion);
            return cacheVersion;
        }
    }

    public async Task ReloadAll()
    {
        await ReloadSites();
        await ReloadFunctions();
        await ReloadGroups();
        await ReloadSiteMenu();
        await ReloadUserSiteLevels();
        await ReloadUserFunctionPolicy();
        await ReloadUsers();
    }

    public async Task ReloadSites()
    {
        var sites = await repository.GetSites();

        await cache.SetAsync(NapApiCacheKey.Sites, sites, cacheOptions);
        await SaveCacheVersion();
    }

    public async Task ReloadFunctions()
    {
        var functionList = (await repository.GetFunctions()).Select(f => f.ToNonNullFunction())
            .ToList();
        await cache.SetAsync(NapApiCacheKey.Functions, functionList, cacheOptions);
        await SaveCacheVersion();
    }

    public async Task ReloadGroups()
    {
        var groupList = (await repository.GetGroups()).Select(u => u.ToNonNullGroup())
            .ToList();

        await cache.SetAsync(NapApiCacheKey.Groups, groupList, cacheOptions);
        await SaveCacheVersion();
    }

    public async Task ReloadUserSiteLevels()
    {
        var siteLevelList = await repository.GetUserSiteLevels();

        await cache.SetAsync(NapApiCacheKey.UserSiteLevels, siteLevelList, cacheOptions);
        await SaveCacheVersion();
    }

    public async Task ReloadUserFunctionPolicy(int siteId = 0)
    {
        var policies = await repository.GetUserFunctionPolicies(siteId);
        var policiesBySite = policies.GroupBy(p => p.SiteId)
            .ToDictionary(p => p.Key, p => p.ToList());
        foreach (var id in policiesBySite.Keys)
        {
            await cache.SetAsync(NapApiCacheKey.UserFunctionPolicies + id, policiesBySite[id], cacheOptions);
        }
        await SaveCacheVersion();
    }

    public async Task ReloadSiteMenu()
    {
        var menuItems = (await repository.GetAllMenuItems())
            .Select(m => m.ToNonNullMenuItem())
            .ToList();
        await cache.SetAsync(NapApiCacheKey.SiteMenuItems, menuItems, cacheOptions);
        await SaveCacheVersion();
    }

    public async Task ReloadUsers()
    {
        var users = (await repository.GetAllUsers())
            .Select(u => u.ToNonNullUser())
            .ToList();
        await cache.SetAsync(NapApiCacheKey.Users, users, cacheOptions);
        await SaveCacheVersion();
    }

    private async Task SaveCacheVersion()
    {
        var version = $"{Guid.NewGuid()}-{DateTime.Now:yyyy-MM-dd-HH:mm:ss}";
        await cache.SetAsync(CacheVersionKey, version, cacheOptions);
    }
}