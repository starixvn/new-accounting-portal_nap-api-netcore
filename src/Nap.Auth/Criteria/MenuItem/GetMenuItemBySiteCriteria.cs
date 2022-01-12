using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.MenuItem;

public class GetMenuItemBySiteCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetBySite";
    }

    public override bool IsValid()
    {
        return true;
    }
}