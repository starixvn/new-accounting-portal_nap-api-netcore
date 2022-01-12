namespace Nap.Auth;

public static class StringUtility
{
    public static string ToNonNull(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
    }
}