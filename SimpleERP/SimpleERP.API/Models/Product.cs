using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleERP.API.Models
{
    /// <summary>
    /// Product entity for inventory management
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        [Required]
        public int StockQuantity { get; set; }
        
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string SKU { get; set; } = string.Empty; // Stock Keeping Unit
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}