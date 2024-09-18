using Fanex.Data.Repository;
using Nap.Auth.Criteria.Trace;
using Nap.Auth.Criteria.User;
using Nap.Auth.Model;
using ONEbook.OTP.Services;

namespace Nap.Auth.Service;

using System.Globalization;
using DataCachingPull;
using Microsoft.Extensions.Configuration;
using Nap.Auth.DataCachingPush;
using ONEbook.UM.Services;

public interface UserService
{
    Task ReUserCheckInAsync(UserReCheckInUserCommand command);

    Task UserCheckInAsync(UserCheckInUserCommand command);

    Task UserLogOutAsync(LogOutUserCommand command);

    Task UserSsoCheckInAsync(UserSsoCheckInUserCommand command);

    Task<string> UserSsoGetTimeAsync();

    Task<bool> ChangePasswordAsync(int siteId, int userId, string password, string loginIp);

    Task<User> GetByIdAsync(int id);

    Task<User> GetByUsernameAsync(string username);

    Task<IEnumerable<User>> GetBySiteAsync(int siteId = 0, bool? isActive = true);

    Task<int> GetUserSiteLevelAsync(int userId, int siteId);

    Task<int> GetUserSiteFunctionKeyPolicyAsync(int userId, int siteId, string functionKey);

    Task<bool> IgnoredOtpAsync(int userId, string system);

    Task<bool> VerifyOtpAsync(int userId, string system, string otp);
}

public class UserServiceImp(
    IConfiguration configuration,
    IMaintenanceService maintenanceService,
    IDynamicRepository repository,
    MemoryCachingStorage cachingService,
    CachingReloaderService dataCachingService,
    OtpService otpService) : UserService
{
    #region Caching User

    public async Task<User> GetByIdAsync(int id)
    {
        if (await maintenanceService.IsUnderMaintenanceAsync())
        {
            return cachingService.GetUser(id);
        }

        var criteria = new GetByIdUserCriteria
        {
            UserId = id
        };

        var user = await repository.GetAsync<User>(criteria).ConfigureAwait(false) ?? new User();

        return user.ToNonNullUser();
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        if (await maintenanceService.IsUnderMaintenanceAsync())
        {
            return cachingService.GetUser(username);
        }

        var criteria = new GetByUsernameUserCriteria
        {
            Username = username
        };

        var user = await repository.GetAsync<User>(criteria).ConfigureAwait(false) ?? new User();

        return user.ToNonNullUser();
    }

    #endregion

    #region User NoCache

    public async Task<IEnumerable<User>> GetBySiteAsync(int siteId = 0, bool? isActive = default)
    {
        var criteria = new GetBySiteCriteria
        {
            SiteId = siteId,
            IsActive = isActive
        };

        var users = await repository.FetchAsync<User>(criteria).ConfigureAwait(false);

        return users.Select(u => u.ToNonNullUser());
    }

    #endregion

    #region SiteLevel

    public async Task<int> GetUserSiteLevelAsync(int userId, int siteId)
    {
        var siteLevels = cachingService.GetUserLevels(userId)
            .Where(s => s.IsActive && s.SiteId == siteId)
            .Select(s => s.Level)
            .ToArray();
        var level = siteLevels.Length > 0 ? siteLevels.Max() : 0;
        return await Task.FromResult(level);
    }

    public async Task<int> GetUserSiteFunctionKeyPolicyAsync(int userId, int siteId, string functionKey)
    {
        var policies = cachingService.GetUserFunctions(userId, siteId)
            .Where(f => f.Key == functionKey)
            .Select(f => f.Policy)
            .ToArray();

        var policy = policies.Length > 0 ? policies.Max() : 0;
        return await Task.FromResult(policy);
    }

    #endregion

    #region OTP

    public async Task<bool> IgnoredOtpAsync(int userId, string system)
    {
        if (await maintenanceService.IsUnderMaintenanceAsync())
        {
            return true;
        }

        var userOtp = await GetUserOtpAsync(userId, system);

        return userOtp == null || userOtp.IgnoreOtp;
    }

    public async Task<bool> VerifyOtpAsync(int userId, string system, string otp)
    {
        if (await maintenanceService.IsUnderMaintenanceAsync())
        {
            return true;
        }

        var userOtp = await GetUserOtpAsync(userId, system);

        if (userOtp == null || userOtp.IgnoreOtp)
        {
            return true;
        }

        if (string.IsNullOrEmpty(userOtp.SecretKey) || string.IsNullOrEmpty(otp))
        {
            return false;
        }

        return otpService.VerifyOtp(userOtp.SecretKey, otp);
    }

    private async Task<UserOtp> GetUserOtpAsync(int userId, string system)
    {
        var criteria = new UserGetOtpCriteria
        {
            CustId = userId,
            System = system
        };

        return await repository.GetAsync<UserOtp>(criteria);
    }

    #endregion

    #region SSO Time

    public async Task<string> UserSsoGetTimeAsync()
    {
        var command = new UserSsoGetTimeUserCommand
        {
            Time = DateTime.Now
        };

        await repository.ExecuteAsync(command).ConfigureAwait(false);

        return command.Time.ToString(CultureInfo.InvariantCulture);
    }

    #endregion

    #region Update

    public async Task<bool> ChangePasswordAsync(int siteId, int userId, string password, string loginIp)
    {
        var command = new UpdatePasswordCustomerCommand
        {
            CurrentUserId = userId,
            CustId = userId,
            CurrentSiteId = siteId,
            LogDate = DateTime.Now,
            UserPwd = password,
            LoginIp = loginIp

        };

        await repository.ExecuteAsync(command).ConfigureAwait(false);
        await dataCachingService.ReloadUsers();
        return command.ErrCode == 0;
    }

    private async Task InsertLogUserLogin(UserCheckInUserCommand command)
    {
        var logCommand = new InsertLogUserLoginCommand
        {
            UserId = command.UserId,
            SiteId = command.SiteId,
            LogDate = command.LoginDate,
            DomainName = command.Domain,
            BrowserName = command.BrowserName,
            Platform = command.Platform,
            Doing = command.Doing,
            IP = command.Ip,
            CountryCode = command.CountryCode,
            Duration = command.Duration
        };

        await ExecuteLoginLogCommand(logCommand).ConfigureAwait(false);
    }

    private async Task InsertLogUserLogin(UserSsoCheckInUserCommand command)
    {
        var logCommand = new InsertLogUserLoginCommand
        {
            UserId = command.UserId,
            SiteId = command.SiteId,
            LogDate = command.LoginDate,
            DomainName = command.Domain,
            BrowserName = command.BrowserName,
            Platform = command.Platform,
            Doing = command.Doing,
            IP = command.IP,
            CountryCode = command.CountryCode,
            Duration = command.Duration
        };

        await ExecuteLoginLogCommand(logCommand).ConfigureAwait(false);
    }

    private async Task ExecuteLoginLogCommand(INonQueryCommand logCommand)
    {
        await repository.ExecuteAsync(logCommand).ConfigureAwait(false);
    }

    #endregion

    #region Checkin

    public async Task UserCheckInAsync(UserCheckInUserCommand command)
    {
        if (!command.IsValid())
        {
            return;
        }

        if (configuration.GetValue<int>("DisableDbLog") == 1)
        {
            command.OnUserId = command.UserId;
            return;
        }

        await repository.ExecuteAsync(command).ConfigureAwait(false);
        await InsertLogUserLogin(command).ConfigureAwait(false);
    }

    public async Task ReUserCheckInAsync(UserReCheckInUserCommand command)
    {
        if (configuration.GetValue<int>("DisableDbLog") == 1)
        {
            return;
        }

        await repository.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task UserLogOutAsync(LogOutUserCommand command)
    {
        if (configuration.GetValue<int>("DisableDbLog") == 1)
        {
            return;
        }

        await repository.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task UserSsoCheckInAsync(UserSsoCheckInUserCommand command)
    {
        if (configuration.GetValue<int>("DisableDbLog") == 1)
        {
            command.OnUserId = command.UserId;
            return;
        }

        await repository.ExecuteAsync(command).ConfigureAwait(false);
        await InsertLogUserLogin(command).ConfigureAwait(false);
    }

    #endregion
}