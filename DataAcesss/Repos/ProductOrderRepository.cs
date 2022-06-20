using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos
{
    public class ProductOrderRepository : IProductOrderRepository
    {
        private readonly PUSGS_ProjectContext _context;

        public ProductOrderRepository(PUSGS_ProjectContext context)
        {
            _context = context;
        }
        public List<ProductOrder> ReadAll()
        {
            return _context.ProductOrders.ToList();
        }

        public List<ProductOrder> ReadForOrder(int orderId)
        {
            var result = _context.ProductOrders.Where(x => x.OrderId == orderId);
            return result.ToList();
        }
    }
}
