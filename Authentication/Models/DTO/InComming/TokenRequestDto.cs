using System.ComponentModel.DataAnnotations;


namespace Authentication;

public class TokenRequestDto
{
    [Required]
    public string Token { get; set; }
    [Required]
    public string RefreashToken { get; set; }
}
