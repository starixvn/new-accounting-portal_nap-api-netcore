namespace Nap.Auth.Model;

public class UserOtp
{
    public int CustId { get; set; }

    public string Username { get; set; }

    public string SecretKey { get; set; }

    public bool IgnoreOtp { get; set; }

    public string System { get; set; }
}