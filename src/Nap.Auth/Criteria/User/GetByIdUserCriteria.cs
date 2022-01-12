using Fanex.Data.Repository;
namespace Nap.Auth.Criteria.User;

public class GetByIdUserCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}