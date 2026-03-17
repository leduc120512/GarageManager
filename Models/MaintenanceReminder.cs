using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class MaintenanceReminder
    {
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = null!;

        [Required]
        public DateTime ReminderDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Vehicle? Vehicle { get; set; }
    }
}