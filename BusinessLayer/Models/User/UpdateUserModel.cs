namespace BusinessLayer;

public class UpdateUserModel
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public DateTime DateOfBirth { get; set; }
    public int Status { get; set; }
    public string MobileNumber { get; set; }
    public string Sex { get; set; }
}
