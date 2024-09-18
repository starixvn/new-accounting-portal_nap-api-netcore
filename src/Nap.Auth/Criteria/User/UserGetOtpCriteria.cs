using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.User;

public class UserGetOtpCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public string System { get; set; }

    public override string GetSettingKey() => "NAP_Admin_GetOTP";

    public override bool IsValid() => true;
}