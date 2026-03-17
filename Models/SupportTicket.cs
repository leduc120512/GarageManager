using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Subject { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? RespondedAt { get; set; }

        [StringLength(1000)]
        public string? Response { get; set; }

        public bool IsResolved { get; set; } = false;

        public string Status { get; set; } = "Pending"; // Pending, InProgress, Resolved

        [StringLength(50)]
        public string? Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent

        public ApplicationUser? Customer { get; set; }
    }
}
