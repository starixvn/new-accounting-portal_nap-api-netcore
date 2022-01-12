using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.User;

public class GetUserSiteFunctionKeyPolicyUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string FunctionKey { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetUserSiteFunctionKeyPolicy";
    }

    public override bool IsValid()
    {
        return true;
    }
}