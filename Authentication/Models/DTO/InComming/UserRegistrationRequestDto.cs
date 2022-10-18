using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string Sex { get; set; }

        public Guid IdentityId { get; set; }
    }
}
