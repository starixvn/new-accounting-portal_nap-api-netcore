namespace Nap.Auth.Model;

public class Site
{
    public byte Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public Site ToNonNullSite()
    {
        return new Site
        {
            IsActive = IsActive,
            Id = Id,
            Name = StringUtility.ToNonNull(Name)
        };
    }
}