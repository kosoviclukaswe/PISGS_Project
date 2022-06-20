using DataAcesss.Constants;
using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PUSGS_Project.Models
{
    public class NewOrderModel
    {
        [Required]
        public string Address { get; set; }
        public string Comment { get; set; }

        public const double FixedPrice  = Constants.DeliveryPrice;
        [Required]
        public double Price { get; set; } = Constants.DeliveryPrice;
        [Required]
        public string Products { get; set; } /*...|ProductA*2|ProductB*1...*/
    }
}
