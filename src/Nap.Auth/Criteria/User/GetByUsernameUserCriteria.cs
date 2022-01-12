using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.User;

public class GetByUsernameUserCriteria : CriteriaBase
{
    public string Username { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetByUsername";
    }

    public override bool IsValid()
    {
        return true;
    }
}