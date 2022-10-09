using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace farm2plate.Models
{
    public class OrderStatusChange
    {
        [Key]
        public int OrderStatusChangeID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string ShopName { get; set; }
        public int SOrderID { get; set; }
        public Status OldSOrderStatus { get; set; }
        public Status NewSOrderStatus { get; set; }
        public DateTime SOrderDateTime { get; set; }


    }
}
