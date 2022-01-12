using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.User;

public class UserSsoGetTimeUserCommand : NonQueryCommand
{
    public DateTime Time { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_SSOGetTime";
    }

    public override bool IsValid()
    {
        return true;
    }
}