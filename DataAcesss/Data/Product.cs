using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Data
{
    public class Product
    {
        public Product()
        {
            ProductOrders = new List<ProductOrder>();
        }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string Ingridients { get; set; }
        public virtual IList<ProductOrder> ProductOrders{ get; set; }
    }
}
