namespace Nap.Auth.Model;

using Business;

public class Function
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public int ParentId { get; set; }

    public string ParentName { get; set; } = string.Empty;

    public int SiteId { get; set; } = 0;

    public string SiteName { get; set; } = string.Empty;

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; } = Constant.Uat;

    public Function ToNonNullFunction()
    {
        return new Function
        {
            Id = Id,
            ParentId = ParentId,
            DisplayOrder = DisplayOrder,
            IsActive = IsActive,
            Name = StringUtility.ToNonNull(Name),
            Key = StringUtility.ToNonNull(Key),
            SiteId= SiteId,
            SiteName = SiteName,
            ParentName = StringUtility.ToNonNull(ParentName),
            Environment = StringUtility.ToNonNull(Environment)
        };
    }
}