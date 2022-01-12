using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.Site;

public class GetByUserSiteCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public string Environment { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetByUser";
    }

    public override bool IsValid()
    {
        return true;
    }
}