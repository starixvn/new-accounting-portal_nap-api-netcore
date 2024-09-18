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

    public MenuItem ToNonNullMenuItem(int functionPolicy)
    {
        return new MenuItem
        {
            Id = Id,
            IsActive = IsActive,
            DisplayOrder = DisplayOrder,
            Policy = functionPolicy,
            ParentId = ParentId,
            Name = StringUtility.ToNonNull(Name),
            Environment = StringUtility.ToNonNull(Environment),
            Url = StringUtility.ToNonNull(Url),
            FunctionName = StringUtility.ToNonNull(FunctionName),
            ParentName = StringUtility.ToNonNull(ParentName)
        };
    }
}