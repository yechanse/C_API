using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleERP.API.Models
{
    /// <summary>
    /// Order entity for order management
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ShippedDate { get; set; }
        
        public DateTime? DeliveredDate { get; set; }
        
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties (제거하여 문제 해결)
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}