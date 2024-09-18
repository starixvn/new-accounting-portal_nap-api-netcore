namespace Nap.Auth.DataCachingPull;

using Fanex.Caching;
using Fanex.Logging;
using Microsoft.Extensions.Hosting;
using Nap.Auth.Model;

public class DataCachingRealTimeJob(ICache cache, MemoryCachingStorage storage) : BackgroundService
{
    private string cacheVersion = string.Empty;

    private List<int> siteIds = [];

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await TryLoadAllCaches(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task TryLoadAllCaches(CancellationToken stoppingToken)
    {
        try
        {
            await LoadAllCaches(stoppingToken);
        }
        catch (Exception e)
        {
            const string message = "DataCachingRealTimeJob - NAP - API. TryLoadCache";
            await LogService.ErrorAsync(message, e);
        }
    }

    private async Task LoadAllCaches(CancellationToken stoppingToken)
    {
        var (isNewCache, newVersion) = await HasNewCacheVersion();
        if (isNewCache)
        {
            await LoadSites();
            await LoadGroups();
            await LoadFunctions();
            await LoadMenuItems();
            await LoadUserSiteLevels();
            await LoadUsers();
            await LoadUserOtp();
            await LoadUserFunctionPolicies();

            CalculateUserMenuItemPolicies();

            cacheVersion = newVersion;
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task<(bool isNew, string version)> HasNewCacheVersion()
    {
        var (found, version) = await cache.TryGetAsync<string>(NapApiCacheKey.CacheVersion);

        return (found && cacheVersion != version, version);
    }

    private async Task LoadSites()
    {
        var (found, sites) = await cache.TryGetAsync<List<int>>(NapApiCacheKey.Sites);
        if (found)
        {
            sites ??= [];
            siteIds = sites;
        }
    }

    private async Task LoadFunctions()
    {
        var (found, functions) = await cache.TryGetAsync<List<Function>>(NapApiCacheKey.Functions);
        if (found)
        {
            functions ??= [];
            storage.SetFunctions(functions);
        }
    }

    private async Task LoadGroups()
    {
        var (found, groups) = await cache.TryGetAsync<List<Group>>(NapApiCacheKey.Groups);
        if (found)
        {
            groups ??= [];
            storage.SetGroups(groups);
        }
    }

    private async Task LoadUserSiteLevels()
    {
        var (found, siteLevels) = await cache.TryGetAsync<List<BatchUserSiteLevel>>(NapApiCacheKey.UserSiteLevels);
        if (found)
        {
            siteLevels ??= [];
            storage.SetUserSiteLevels(siteLevels);
        }
    }

    private async Task LoadUserFunctionPolicies()
    {
        foreach (var siteId in siteIds)
        {
            var (found, policies) = await cache.TryGetAsync<List<BatchUserFunctionPolicy>>(NapApiCacheKey.UserFunctionPolicies + siteId);
            if (found)
            {
                policies ??= [];
                storage.SetUserFunctionPolicies(siteId, policies);
            }
        }
    }

    private async Task LoadMenuItems()
    {
        var (found, menuItems) = await cache.TryGetAsync<List<BatchMenuItem>>(NapApiCacheKey.SiteMenuItems);
        if (found)
        {
            menuItems ??= [];
            storage.SetMenuItems(menuItems);
        }
    }

    private void CalculateUserMenuItemPolicies()
    {
        storage.CalculateUserMenuItemPolicies();
    }

    private async Task LoadUserOtp()
    {
        var (found, users) = await cache.TryGetAsync<List<UserOtp>>(NapApiCacheKey.UserOTP);
        if (found)
        {
            users ??= [];
            storage.SetUserOtp(users);
        }
    }

    private async Task LoadUsers()
    {
        var (found, users) = await cache.TryGetAsync<List<User>>(NapApiCacheKey.Users);
        if (found)
        {
            users ??= [];
            storage.SetUsers(users);
        }
    }
}
