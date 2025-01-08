namespace Nap.Auth.DataCachingPull;

using Model;
using Model.ValueObject;

public class BatchUserFunctionPolicy
{
    public int SiteId { get; set; }

    public int UserId { get; set; }

    public int FunctionId { get; set; }

    public string FunctionKey { get; set; }

    public string FunctionName { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public int Policy { get; set; }

    public string Environment { get; set; }

    public string ParentKey { get; set; }

    public FunctionPolicy ToFunctionPolicy()
    {
        return new FunctionPolicy
        {
            Key = FunctionKey,
            Id = FunctionId,
            Name = FunctionName,
            ParentId = ParentId,
            ParentKey = ParentKey,
            Policy = Policy
        };
    }
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


    public MenuItem ToMenuItem()
    {
        return new MenuItem
        {
            Id = Id,
            Name = Name,
            ParentId = ParentId,
            Url = Url,
            IsActive = IsActive,
            Environment = Environment,
            Policy = Policy,
            DisplayOrder = DisplayOrder,
            ParentName = ParentName,
            FunctionName = FunctionName,
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