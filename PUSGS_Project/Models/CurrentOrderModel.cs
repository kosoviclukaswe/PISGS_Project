using DataAcesss.Constants;
using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PUSGS_Project.Models
{
    public class CurrentOrderModel
    {
        public Dictionary<Product, int> Products { get; set; }
        public int Id { get; set; }
        public string Address { get; set; }
        public double Price { get; set; }
        public const double FixedPrice = Constants.DeliveryPrice;
        public string Comment { get; set; }
        public int Time { get; set; }
        public OrderState Status { get; set; }
    }
}
