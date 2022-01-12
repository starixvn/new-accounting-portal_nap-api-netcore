using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.Trace;

public class InsertLogUserLoginCommand : NonQueryCommand
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string IP { get; set; }

    public DateTime LogDate { get; set; }

    public string Doing { get; set; }

    public string DomainName { get; set; }

    public string BrowserName { get; set; }

    public string Platform { get; set; }

    public string CountryCode { get; set; }

    public int Duration { get; set; }

    public override bool IsValid()
    {
        return true;
    }

    public override string GetSettingKey()
    {
        return "NAP_LogUserLogin_Insert";
    }
}