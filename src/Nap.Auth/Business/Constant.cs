namespace Nap.Auth.Business;

public static class Constant
{
    public const string Uat = "UAT";
    public const string Live = "LIVE";
    public const string It = "IT";

    public enum ReCheckInResult
    {
        Ok = 0,
        DownTime = 1,
        KickedOut = 2
    }
}
