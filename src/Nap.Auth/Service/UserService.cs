using Fanex.Data.Repository;
using Fanex.Logging;
using Nap.Auth.Criteria.Trace;
using Nap.Auth.Criteria.User;
using Nap.Auth.Model;

namespace Nap.Auth.Service;

public interface UserService
{
    Task ReUserCheckIn(UserReCheckInUserCommand command);

    Task UserCheckIn(UserCheckInUserCommand command);

    Task UserLogOut(LogOutUserCommand command);

    Task UserSsoCheckIn(UserSSOCheckInUserCommand command);

    Task<string> UserSsoGetTime();

    Task<bool> ChangePassword(int siteId, int userId, string password, string loginIp);

    Task<User> GetById(int id);

    Task<User> GetByUsername(string username);

    Task<IEnumerable<User>> GetByFunctionAndSite(int functionId, int siteId = 0);

    Task<int> GetUserSiteLevel(int userId, int siteId);

    Task<int> GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey);
}

public class UserServiceImp : UserService
{
    private readonly IDynamicRepository repository;

    public UserServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<User> GetById(int id)
    {
        var criteria = new GetByIdUserCriteria
        {
            UserId = id
        };

        var user = await repository.GetAsync<User>(criteria).ConfigureAwait(false) ?? new User();

        return user.ToNonNullUser();
    }

    public async Task<User> GetByUsername(string username)
    {
        var criteria = new GetByUsernameUserCriteria
        {
            Username = username
        };

        var user = await repository.GetAsync<User>(criteria).ConfigureAwait(false) ?? new User();

        return user.ToNonNullUser();
    }

    public async Task<IEnumerable<User>> GetByFunctionAndSite(int functionId, int siteId = 0)
    {
        var criteria = new GetByFunctionAndSiteUserCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        var users = await repository.FetchAsync<User>(criteria).ConfigureAwait(false);

        return users.Select(u => u.ToNonNullUser());
    }


    public async Task<int> GetUserSiteLevel(int userId, int siteId)
    {
        var criteria = new GetUserSiteLevelUserCriteria
        {
            UserId = userId,
            SiteId = siteId
        };

        return await repository.GetAsync<int>(criteria);
    }

    public async Task<bool> ChangePassword(int siteId, int userId, string password, string loginIp)
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

        return command.ErrCode == 0;
    }

    public async Task UserCheckIn(UserCheckInUserCommand command)
    {
        if (!command.IsValid())
        {
            return;
        }

        await repository.ExecuteAsync(command).ConfigureAwait(false);
        await InsertLogUserLogin(command).ConfigureAwait(false);
    }

    public async Task ReUserCheckIn(UserReCheckInUserCommand command)
    {
        await repository.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task UserLogOut(LogOutUserCommand command)
    {
        await repository.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task UserSsoCheckIn(UserSSOCheckInUserCommand command)
    {
        await repository.ExecuteAsync(command).ConfigureAwait(false);
        await InsertLogUserLogin(command).ConfigureAwait(false);
    }

    public async Task<int> GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey)
    {
        var criteria = new GetUserSiteFunctionKeyPolicyUserCriteria
        {
            UserId = userId,
            SiteId = siteId,
            FunctionKey = functionKey
        };

        return await repository.GetAsync<int>(criteria).ConfigureAwait(false);
    }

    public async Task<string> UserSsoGetTime()
    {
        var command = new UserSsoGetTimeUserCommand
        {
            Time = DateTime.Now
        };

        await repository.ExecuteAsync(command).ConfigureAwait(false);

        return command.Time.ToString();
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

    private async Task InsertLogUserLogin(UserSSOCheckInUserCommand command)
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

    private async Task ExecuteLoginLogCommand(InsertLogUserLoginCommand logCommand)
    {
        try
        {
            await repository.ExecuteAsync(logCommand).ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            await Logger.Log.ErrorAsync(ex.Message, ex).ConfigureAwait(false);
        }
    }
}