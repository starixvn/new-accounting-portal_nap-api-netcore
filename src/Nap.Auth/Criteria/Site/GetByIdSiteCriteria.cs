using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.Site;

public class GetByIdSiteCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}