using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class ServiceHistory
    {
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public string MechanicId { get; set; } = null!;

        [Required]
        public DateTime ServiceDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }

        public Vehicle? Vehicle { get; set; }
        public ApplicationUser? Mechanic { get; set; }
        public ICollection<ServiceHistoryService> ServicesPerformed { get; set; } = new List<ServiceHistoryService>();
        public ICollection<ServiceHistoryPart> PartsUsed { get; set; } = new List<ServiceHistoryPart>();
    }
}