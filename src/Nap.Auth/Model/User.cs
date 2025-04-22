namespace Nap.Auth.Model;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public int GroupId { get; set; }

    public DateTime PasswordExpiry { get; set; }

    public User ToNonNullUser()
    {
        return new User
        {
            Id = Id,
            GroupId = GroupId,
            Name = StringUtility.ToNonNull(Name),
            Username = StringUtility.ToNonNull(Username),
            Password = StringUtility.ToNonNull(Password),
            FirstName = StringUtility.ToNonNull(FirstName),
            LastName = StringUtility.ToNonNull(LastName),
            PasswordExpiry = PasswordExpiry,
            Email = StringUtility.ToNonNull(Email)
        };
    }
}