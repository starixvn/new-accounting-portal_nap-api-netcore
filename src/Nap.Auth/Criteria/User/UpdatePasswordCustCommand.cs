using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.User;

public class UpdatePasswordCustomerCommand : NonQueryCommand
{
    public int CustId { get; set; }

    public string UserPwd { get; set; }

    public int CurrentUserId { get; set; }

    public int CurrentSiteId { get; set; }

    public DateTime LogDate { get; set; }

    public string LoginIp { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public DateTime PasswordExpiry { get; set; } = DateTimeOffset.Now.Date.AddMonths(1);

    public override string GetSettingKey()
    {
        return "NAP_Cust_UpdatePassword";
    }

    public override bool IsValid()
    {
        return true;
    }
}