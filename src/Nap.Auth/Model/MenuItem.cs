namespace Nap.Auth.Model;

public class MenuItem
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
            if (!string.IsNullOrEmpty(this.Url))
            {
                return this.Url.StartsWith("/", StringComparison.CurrentCulture) ? "~" + this.Url : this.Url;
            }

            return "/";
        }

    }

    public string FunctionName { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public MenuItem ToNonNullMenuItem()
    {
        return new MenuItem
        {
            Id = Id,
            IsActive = IsActive,
            DisplayOrder = DisplayOrder,
            Policy = Policy,
            ParentId = ParentId,
            Name = StringUtility.ToNonNull(Name),
            Environment = StringUtility.ToNonNull(Environment),
            Url = StringUtility.ToNonNull(Url),
            FunctionName = StringUtility.ToNonNull(FunctionName),
            ParentName = StringUtility.ToNonNull(ParentName)
        };
    }
}