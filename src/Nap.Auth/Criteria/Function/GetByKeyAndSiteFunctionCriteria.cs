using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.Function;

public class GetByKeyAndSiteFunctionCriteria : CriteriaBase
{
    public string FunctionKey { get; set; }

    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetByKeyAndSite";
    }

    public override bool IsValid()
    {
        return true;
    }
}
