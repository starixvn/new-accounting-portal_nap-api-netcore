using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.User;

public class GetBySiteCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public bool? IsActive { get; set; }

    public override string GetSettingKey()=> "NAP_User_GetBySite";

    public override bool IsValid()=>true;
}