using farm2plate.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace farm2plate.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        [Required]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int ShopID { get; set; }
        [ForeignKey("ShopID")]
        public Shop Shop { get; set; }
        
        [Required]
        public Status OrderStatus { get; set; }

    }
}

public enum Status
{
    IN_PROGRESS,
    IN_TRANSIT,
    RECEIVED
}
