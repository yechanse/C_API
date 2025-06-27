using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace SimpleERP.API.Models
{
    /// <summary>
    /// Customer entity (extends User for ERP purposes)
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation property
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}