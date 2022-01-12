namespace Nap.Auth.Model;

public class Group
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public Group ToNonNullGroup()
    {
        return new Group
        {
            IsActive = IsActive,
            Id = Id,
            Name = StringUtility.ToNonNull(Name)
        };
    }
}