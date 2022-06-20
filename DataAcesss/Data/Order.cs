using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Data
{
    public enum OrderState
    {
        COMPLETE,
        INCOMPLETE,
        ON_HOLD,
        ACTIVE
    }
    public class Order
    {
        public Order()
        {
            ProductOrders = new List<ProductOrder>();
        }

        [Required]
        public int OrderId { get; set; }
        [Required]
        public string AppUserId { get; set; }
        [Required]
        public AppUser AppUser { get; set; }
        public string DelivererId { get; set; }
        public AppUser Deliverer { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double FixedPrice { get; set; } = Constants.Constants.DeliveryPrice; 
        public string Comment { get; set; }
        [Required]
        public OrderState OrderState { get; set; }
        [Required]
        public int Time { get; set; } // In minutes
        public virtual IList<ProductOrder> ProductOrders { get; set; }

    }
}
