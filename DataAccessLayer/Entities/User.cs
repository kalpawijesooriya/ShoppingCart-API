namespace DataAccessLayer;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string MobileNumber { get; set; }
    public string Sex { get; set; }
    public Guid IdentityId { get; set; }
}
