using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string RoleName { get; set; } = "User"; // Optional: If you want to assign a role during login
    }
}