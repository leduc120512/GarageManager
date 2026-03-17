using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Models
{
    public class Part
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
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [StringLength(100)]
        public string? Supplier { get; set; }

        public ICollection<ServiceHistoryPart> ServiceHistories { get; set; } = new List<ServiceHistoryPart>();
    }
}