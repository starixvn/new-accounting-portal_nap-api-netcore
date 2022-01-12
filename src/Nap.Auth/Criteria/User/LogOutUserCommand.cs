using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.User;

public class LogOutUserCommand : NonQueryCommand
{
    public int OnUserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_Logout";
    }

    public override bool IsValid()
    {
        return true;
    }
}