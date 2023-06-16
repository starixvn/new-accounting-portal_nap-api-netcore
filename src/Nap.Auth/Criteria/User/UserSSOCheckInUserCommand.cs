
using Fanex.Data;
using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.User;

public class UserSsoCheckInUserCommand : NonQueryCommand
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string NetSessionId { get; set; }

    public int CheckOnUserId { get; set; }

    public int OnUserId { get; set; }

    public string IP { get; set; }

    public DateTime LoginDate { get; set; }

    [SpParam(Ignored = true)]
    public string Doing { get; set; }

    [SpParam(Ignored = true)]
    public string Domain { get; set; }

    [SpParam(Ignored = true)]
    public string BrowserName { get; set; }

    [SpParam(Ignored = true)]
    public string Platform { get; set; }

    [SpParam(Ignored = true)]
    public string CountryCode { get; set; }

    [SpParam(Ignored = true)]
    public int Duration { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_SSOCheckIn";
    }

    public override bool IsValid()
    {
        return true;
    }
}