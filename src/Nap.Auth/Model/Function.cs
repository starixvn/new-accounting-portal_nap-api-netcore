namespace Nap.Auth.Model;

public class Function
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; }

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
            ParentName = StringUtility.ToNonNull(ParentName),
            Environment = StringUtility.ToNonNull(Environment)
        };
    }
}