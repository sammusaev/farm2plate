using farm2plate.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace farm2plate.Models
{
    public class SOrder
    {
        [Key]
        public int SOrderID { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        [Required]
        [ForeignKey("ProductID")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        #nullable enable
        public string? UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser? ApplicationUser { get; set; }

        #nullable enable
        [ForeignKey("ShopID")]
        public int? ShopID { get; set; }
        public Shop? Shop { get; set; }
        
        [Required]
        public Status SOrderStatus { get; set; }

    }
}

public enum Status
{
    IN_PROGRESS,
    IN_TRANSIT,
    RECEIVED,
    CANCELLED
}
