using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class Settings
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string StoreName { get; set; } = "Auto Garage Manager";

        [StringLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public bool EnableEmailNotifications { get; set; } = true;

        public bool AllowUserRegistration { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
