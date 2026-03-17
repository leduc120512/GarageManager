using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class ServiceHistoryPart
    {
        public int Id { get; set; }

        [Required]
        public int ServiceHistoryId { get; set; }

        [Required]
        public int PartId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public ServiceHistory? ServiceHistory { get; set; }
        public Part? Part { get; set; }
    }
}