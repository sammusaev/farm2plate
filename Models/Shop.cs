using farm2plate.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace farm2plate.Models
{
    public class Shop
    {
        [Required]
        public int ShopID { get; set; }

        [Required]
        public string Name { get; set; }

        // https://stackoverflow.com/questions/60062443/identity-user-fk
        [Required]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<SOrder> SOrders { get; set; }
    }
}
