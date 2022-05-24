namespace Nap.Auth.Model.ValueObject;

public class FunctionPolicy
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public int Policy { get; set; }

    public int ParentId { get; set; }

    public string ParentKey { get; set; }

    public string Description { get; set; }

    public FunctionPolicy ToNonNullFunctionPolicy()
    {
        return new FunctionPolicy
        {
            Id = Id,
            Policy = Policy,
            ParentId = ParentId,
            Name = StringUtility.ToNonNull(Name),
            Key = StringUtility.ToNonNull(Key),
            ParentKey = StringUtility.ToNonNull(ParentKey),
            Description = StringUtility.ToNonNull(Description)
        };
    }
}