using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int EstimatedDuration { get; set; } // in minutes

        public ICollection<ServiceHistoryService> ServiceHistories { get; set; } = new List<ServiceHistoryService>();
    }
}