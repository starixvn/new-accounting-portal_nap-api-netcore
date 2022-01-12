using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.Function;

public class GetByUserAndSiteIncludeGroupFunctionCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetByUserAndSiteIncludeGroup";
    }

    public override bool IsValid()
    {
        return true;
    }
}