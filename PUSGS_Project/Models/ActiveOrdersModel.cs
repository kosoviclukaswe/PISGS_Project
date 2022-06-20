using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PUSGS_Project.Models
{
    public class ActiveOrdersModel
    {
        public List<Order> Orders { get; set; }
        public Dictionary<int, Dictionary<Product, int>> Products { get; set; }
    }
}
