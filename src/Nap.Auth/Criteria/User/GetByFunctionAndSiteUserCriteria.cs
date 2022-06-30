using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.User;

public class GetByFunctionAndSiteUserCriteria : CriteriaBase
{
    public int FunctionId { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetUserByFunctionAndSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}