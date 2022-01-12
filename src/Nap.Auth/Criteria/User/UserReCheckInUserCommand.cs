using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.User;

public class UserReCheckInUserCommand : NonQueryCommand
{
    public int OnUserId { get; set; }

    public bool IsKickedOut { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_ReCheckIn";
    }

    public override bool IsValid()
    {
        return true;
    }
}