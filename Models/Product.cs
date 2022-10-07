using farm2plate.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace farm2plate.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [Display(Name="Product Name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name="Price (RM)")]
        public double ProductPrice { get; set; }

        [Required]
        [Display(Name="Product Image")]
        public string ProductImage { get; set; }

        [Required]
        [Display(Name="Quantity Left (kg)")]
        public int ProductQuantity { get; set; }

        [Required]
        public int ShopID { get; set; }
        [ForeignKey("ShopID")]
        public virtual Shop Shop { get; set; }
        
    }
}
