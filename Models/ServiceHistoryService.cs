using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class ServiceHistoryService
    {
        public int Id { get; set; }

        [Required]
        public int ServiceHistoryId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public ServiceHistory? ServiceHistory { get; set; }
        public Service? Service { get; set; }
    }
}