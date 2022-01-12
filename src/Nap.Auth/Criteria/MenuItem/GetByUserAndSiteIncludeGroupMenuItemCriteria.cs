using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.MenuItem;

public class GetByUserAndSiteIncludeGroupMenuItemCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetByUserAndSiteIncludeGroup";
    }

    public override bool IsValid()
    {
        return true;
    }
}