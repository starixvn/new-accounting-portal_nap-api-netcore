using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.Group;

public class GetByIdGroupCriteria : CriteriaBase
{
    public int GroupId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Group_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}