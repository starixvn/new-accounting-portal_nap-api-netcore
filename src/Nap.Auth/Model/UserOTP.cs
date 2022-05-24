namespace Nap.Auth.Model;

public class UserOTP
{
    public int CustId { get; set; }

    public string Username { get; set; }

    public string SecretKey { get; set; }

    public bool IgnoreOTP { get; set; }

    public string System { get; set; }
}