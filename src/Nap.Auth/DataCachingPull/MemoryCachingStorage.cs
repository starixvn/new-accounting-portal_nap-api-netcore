namespace Nap.Auth.DataCachingPull;

using Business;
using Model;
using Model.ValueObject;

public interface MemoryCachingStorage
{
    void SetFunctions(List<Function> functions);
    void SetGroups(List<Group> groups);
    void SetUserSiteLevels(List<BatchUserSiteLevel> siteLevels);
    void SetUserFunctionPolicies(int siteId, List<BatchUserFunctionPolicy> policies);
    void SetMenuItems(List<BatchMenuItem> menuItems);
    void CalculateUserMenuItemPolicies();
    void SetUserOtp(List<UserOtp> users);
    void SetUsers(List<User> users);
    List<FunctionPolicy> GetUserFunctions(int userId, int siteId, string env = Constant.Uat);
    List<MenuItem> GetUserMenuItems(int userId, int siteId, string env = Constant.Uat);
    int GetFunctionIdByKey(string key, int siteId);
    Group GetGroup(int id);
    List<SiteLevel> GetUserLevels(int userId, string env = Constant.Uat);
    List<MenuItem> GetSiteMenuItems(int siteId, string env = Constant.Uat, bool? isActive = null);
    UserOtp GetUserOtp(int userId, string system);
    User GetUser(int id);
    User GetUser(string username);
    List<User> GetUsers(int siteId, bool? isActive = null);

    IEnumerable<UserFunctionPolicy> GetUserFunctionPolicies(string key, int siteId);

    #region Debug
    Dictionary<int, Dictionary<int, List<BatchUserFunctionPolicy>>> SiteUserFunctionPoliciesCache { get; }
    Dictionary<int, Dictionary<int, List<MenuItem>>> SiteUserMenuItemsCache { get; }
    Dictionary<int, Function> FunctionsCache { get; }
    Dictionary<int, Group> GroupsCache { get; }
    Dictionary<int, List<BatchUserSiteLevel>> UserSiteLevelsByUserCache { get; }
    Dictionary<int, List<BatchUserSiteLevel>> UserSiteLevelsBySiteCache { get; }
    Dictionary<int, List<MenuItem>> SiteMenuItemsCache { get; }
    List<UserOtp> UserOtpCache { get; }
    List<User> UsersCache { get; }

    #endregion
}

public class MemoryCachingStorageImp : MemoryCachingStorage
{
    public void SetFunctions(List<Function> functions)
    {
        FunctionsCache = functions.ToDictionary(f => f.Id, f => f.ToNonNullFunction());
    }

    public void SetGroups(List<Group> groups)
    {
        GroupsCache = groups.ToDictionary(f => f.Id, f => f.ToNonNullGroup());
    }

    public void SetUserSiteLevels(List<BatchUserSiteLevel> siteLevels)
    {
        UserSiteLevelsByUserCache = siteLevels.GroupBy(f => f.UserId)
            .ToDictionary(f => f.Key, f => f.ToList());

        UserSiteLevelsBySiteCache = siteLevels.GroupBy(f => f.SiteId)
            .ToDictionary(f => f.Key, f => f.ToList());
    }

    public void SetUserFunctionPolicies(int siteId, List<BatchUserFunctionPolicy> policies)
    {
        CalculateDerivativeValue(policies);

        var policiesByUsers = policies.GroupBy(p => p.UserId)
            .ToDictionary(p => p.Key, p => p.ToList());
        SiteUserFunctionPoliciesCache[siteId] = policiesByUsers;
    }

    private void CalculateDerivativeValue(List<BatchUserFunctionPolicy> policies)
    {
        var defaultFunction = new Function();
        policies.ForEach(policy =>
        {
            var function = FunctionsCache.GetValueOrDefault(policy.FunctionId, defaultFunction);
            var parentFunction = FunctionsCache.GetValueOrDefault(function.ParentId, defaultFunction);
            policy.Environment = function.Environment;
            policy.FunctionKey = function.Key;
            policy.FunctionName = function.Name;
            policy.ParentId = function.ParentId;
            policy.ParentName = function.ParentName;
            policy.ParentKey = parentFunction.Key;
        });
    }

    public void SetMenuItems(List<BatchMenuItem> menuItems)
    {
        SiteMenuItemsCache = menuItems.GroupBy(f => f.SiteId)
            .ToDictionary(i => i.Key, i => i.Select(f => f.ToMenuItem()).ToList());
    }

    public void CalculateUserMenuItemPolicies()
    {
        var newSiteUserMenuItems = new Dictionary<int, Dictionary<int, List<MenuItem>>>();
        foreach (var siteId in SiteMenuItemsCache.Keys)
        {
            if (!SiteUserFunctionPoliciesCache.TryGetValue(siteId, out var policiesBySite))
            {
                continue;
            }

            var menuItemsBySite = SiteMenuItemsCache[siteId];
            var newUserMenuItemsBySite = new Dictionary<int, List<MenuItem>>();
            foreach (var userId in policiesBySite.Keys)
            {
                var policiesByUser = policiesBySite[userId];

                var menuItemsByUser = policiesByUser
                    .Join(menuItemsBySite, p => p.FunctionId, m => m.FunctionId, (p, m) => new { Menu = m, Function = p })
                    .Where(x => x.Menu.Policy <= x.Function.Policy)
                    .Select(x => x.Menu.ToNonNullMenuItem(x.Function.Policy))
                    .ToList();
                newUserMenuItemsBySite.Add(userId, menuItemsByUser);
            }

            newSiteUserMenuItems.Add(siteId, newUserMenuItemsBySite);
        }

        SiteUserMenuItemsCache = newSiteUserMenuItems;
    }

    public void SetUserOtp(List<UserOtp> users)
    {
        UserOtpCache = users.ToList();
    }

    public void SetUsers(List<User> users)
    {
        UsersCache = users.Select(u => u.ToNonNullUser()).ToList();
    }

    public List<FunctionPolicy> GetUserFunctions(int userId, int siteId, string env = Constant.Uat)
    {
        if (!SiteUserFunctionPoliciesCache.TryGetValue(siteId, out var policiesBySite))
        {
            return [];
        }

        if (!policiesBySite.TryGetValue(userId, out var policiesByUser))
        {
            return [];
        }

        return policiesByUser.Where(s => EnvironmentMatched(s.Environment, env))
            .Select(p => p.ToFunctionPolicy())
            .Select(p => p.ToNonNullFunctionPolicy())
            .ToList();
    }

    public List<MenuItem> GetUserMenuItems(int userId, int siteId, string env = Constant.Uat)
    {
        if (!SiteUserMenuItemsCache.TryGetValue(siteId, out var policiesBySite))
        {
            return [];
        }

        if (!policiesBySite.TryGetValue(userId, out var policiesByUser))
        {
            return [];
        }

        return policiesByUser.Where(s => EnvironmentMatched(s.Environment, env))
            .Where(m => m.IsActive)
            .ToList();
    }

    public IEnumerable<UserFunctionPolicy> GetUserFunctionPolicies(string key, int siteId)
    {
        if (!SiteUserFunctionPoliciesCache.TryGetValue(siteId, out var users))
        {
            yield break;
        }

        var functionId = GetFunctionIdByKey(key, siteId);

        foreach (var userId in users.Keys)
        {
            if (!users[userId].Exists(f => f.FunctionId == functionId))
            {
                continue;
            }
            var policy = users[userId]
                .Where(f => f.FunctionId == functionId)
                .Select(f => f.Policy)
                .Max();
            yield return new UserFunctionPolicy
            {
                UserId = userId,
                FunctionId = functionId,
                Policy = policy
            };
        }
    }

    public int GetFunctionIdByKey(string key, int siteId)
    {
        var function = FunctionsCache.Values.FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase) && f.SiteId == siteId);

        return function?.Id ?? 0;
    }

    public Group GetGroup(int id)
    {
        GroupsCache.TryGetValue(id, out var group);

        return group;
    }

    public List<SiteLevel> GetUserLevels(int userId, string env = Constant.Uat)
    {
        if (!UserSiteLevelsByUserCache.TryGetValue(userId, out var siteLevels))
        {
            return [];
        }

        return siteLevels
            .Where(s => EnvironmentMatched(s.Environment, env))
            .Select(s => new SiteLevel
            {
                SiteId = s.SiteId,
                IsActive = s.IsActive,
                Level = s.Level
            }).ToList();
    }

    public List<MenuItem> GetSiteMenuItems(int siteId, string env = Constant.Uat, bool? isActive = null)
    {
        if (!SiteMenuItemsCache.TryGetValue(siteId, out var menuItems))
        {
            return [];
        }

        return menuItems.Where(s => EnvironmentMatched(s.Environment, env))
            .Where(s => isActive == null || s.IsActive == isActive.Value)
            .ToList();
    }

    public UserOtp GetUserOtp(int userId, string system)
    {
        var otp = UserOtpCache.Find(o => o.CustId == userId && string.Equals(o.System, system, StringComparison.OrdinalIgnoreCase));
        return otp ?? new UserOtp { IgnoreOtp = true };
    }

    public User GetUser(int id)
    {
        var user = UsersCache.Find(u => u.Id == id) ?? new User();

        return user.ToNonNullUser();
    }

    public User GetUser(string username)
    {
        var user = UsersCache.Find(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)) ?? new User();

        return user.ToNonNullUser();
    }

    public List<User> GetUsers(int siteId, bool? isActive = null)
    {
        if (!UserSiteLevelsBySiteCache.TryGetValue(siteId, out var userSiteLevels))
        {
            return [];
        }

        var userIds = userSiteLevels
                .Where(u => isActive == null || u.IsActive == isActive.Value)
                .Select(u => u.UserId);

        return UsersCache.Where(u => userIds.Contains(u.Id))
            .Select(u => u.ToNonNullUser())
            .OrderBy(u => u.Id)
            .ToList();
    }

    private static bool EnvironmentMatched(string checkingEnv, string env)
    {
        return string.Equals(env, Constant.Uat, StringComparison.OrdinalIgnoreCase)
               || string.Equals(checkingEnv, env, StringComparison.OrdinalIgnoreCase);
    }

    #region Debug

    public Dictionary<int, Dictionary<int, List<BatchUserFunctionPolicy>>> SiteUserFunctionPoliciesCache { get; } = [];

    public Dictionary<int, Dictionary<int, List<MenuItem>>> SiteUserMenuItemsCache { get; private set; } = [];

    public Dictionary<int, Function> FunctionsCache { get; private set; } = [];

    public Dictionary<int, Group> GroupsCache { get; private set; } = [];

    public Dictionary<int, List<BatchUserSiteLevel>> UserSiteLevelsByUserCache { get; private set; } = [];

    public Dictionary<int, List<BatchUserSiteLevel>> UserSiteLevelsBySiteCache { get; private set; } = [];

    public Dictionary<int, List<MenuItem>> SiteMenuItemsCache { get; private set; } = [];

    public List<UserOtp> UserOtpCache { get; private set; } = [];

    public List<User> UsersCache { get; private set; } = [];

    #endregion
}