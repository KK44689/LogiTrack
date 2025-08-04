using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LogiTrack.Models
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string RoleName { get; set; } = "User"; // Optional: If you want to assign a role during registration
    }
}