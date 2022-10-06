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
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        // https://stackoverflow.com/questions/60062443/identity-user-fk
        [Required]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
