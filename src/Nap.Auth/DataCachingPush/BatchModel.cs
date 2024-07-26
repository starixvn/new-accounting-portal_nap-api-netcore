namespace Nap.Auth.DataCachingPush;

public class JobCommand(string command, int siteId)
{
    public const string Site = "SiteCommand";
    public const string Function = "FunctionCommand";
    public const string Group = "GroupCommand";
    public const string Menu = "MenuCommand";
    public const string UserFunctionPolicy = "UserFunctionPolicyCommand";
    public const string UserSiteLevel = "UserSiteLevelCommand";
    public const string User = "UserCommand";

    public string Command { get; set; } = command;

    public int SiteId { get; set; } = siteId;
}

public static class Constant
{
    public const string Uat = "UAT";
    public const string Live = "LIVE";
    public const string It = "IT";
}

public class BatchUserFunctionPolicy
{
    public int SiteId { get; set; }

    public int UserId { get; set; }

    public int FunctionId { get; set; }

    public int Policy { get; set; }
}

public class BatchMenuItem
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public short DisplayOrder { get; set; }

    public string Environment { get; set; }

    public string Url { get; set; }

    public int Policy { get; set; }

    public string ValidUrl
    {
        get
        {
            if (!string.IsNullOrEmpty(Url))
            {
                return Url.StartsWith("/", StringComparison.CurrentCulture) ? "~" + Url : Url;
            }

            return "/";
        }

    }

    public int FunctionId { get; set; }

    public string FunctionName { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public int SiteId { get; set; }

    public BatchMenuItem ToNonNullMenuItem()
    {
        return new BatchMenuItem
        {
            Id = Id,
            IsActive = IsActive,
            DisplayOrder = DisplayOrder,
            Policy = Policy,
            ParentId = ParentId,
            SiteId = SiteId,
            Name = StringUtility.ToNonNull(Name),
            Environment = StringUtility.ToNonNull(Environment),
            Url = StringUtility.ToNonNull(Url),
            FunctionName = StringUtility.ToNonNull(FunctionName),
            ParentName = StringUtility.ToNonNull(ParentName),
            FunctionId = FunctionId
        };
    }
}

public class BatchUserSiteLevel
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public int Level { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; }
}

public class BatchGroup
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public BatchGroup ToNonNullGroup()
    {
        return new BatchGroup
        {
            IsActive = IsActive,
            Id = Id,
            Name = StringUtility.ToNonNull(Name)
        };
    }
}

public class BatchUser
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public int GroupId { get; set; }

    public BatchUser ToNonNullUser()
    {
        return new BatchUser
        {
            Id = Id,
            GroupId = GroupId,
            Name = StringUtility.ToNonNull(Name),
            Username = StringUtility.ToNonNull(Username),
            Password = StringUtility.ToNonNull(Password),
            FirstName = StringUtility.ToNonNull(FirstName),
            LastName = StringUtility.ToNonNull(LastName),
            Email = StringUtility.ToNonNull(Email)
        };
    }
}

public class BatchUserOtp
{
    public int CustId { get; set; }

    public string Username { get; set; }

    public string SecretKey { get; set; }

    public bool IgnoreOtp { get; set; }

    public string System { get; set; }
}

public class BatchFunction
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public int SiteId { get; set; }

    public string SiteName { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; }

    public BatchFunction ToNonNullFunction()
    {
        return new BatchFunction
        {
            Id = Id,
            ParentId = ParentId,
            DisplayOrder = DisplayOrder,
            IsActive = IsActive,
            Name = StringUtility.ToNonNull(Name),
            Key = StringUtility.ToNonNull(Key),
            ParentName = StringUtility.ToNonNull(ParentName),
            Environment = StringUtility.ToNonNull(Environment),
            SiteId = SiteId,
            SiteName = StringUtility.ToNonNull(SiteName)
        };
    }
}