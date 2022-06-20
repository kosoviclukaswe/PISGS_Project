using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos.Contracts
{
    public interface IProductOrderRepository
    {
        List<ProductOrder> ReadForOrder(int orderId);
        List<ProductOrder> ReadAll();
    }
}
