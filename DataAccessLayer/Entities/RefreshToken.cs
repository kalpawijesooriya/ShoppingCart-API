using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer;

public class RefreshToken: BaseEntity
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevorked { get; set; }
    public DateTime ExpiryDate { get; set; }

    [ForeignKey (nameof (UserId))]
    public IdentityUser User { get; set; }

}
