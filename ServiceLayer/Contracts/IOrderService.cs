using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Contracts
{
    public interface IOrderService
    {
        Task Create(Order order);
        Order Read(int id);
        List<Order> ReadAll();
        List<Order> ReadAll(string userId);
        List<Order> ReadAllForDeliverer(string delivererId);
        List<Order> ReadAll(OrderState state);
        List<Order> ReadCompletedOrdersByDeliverer(string delivererId);
        int DelivererHasActiveOrder(string delivererId);
        void Update(Order order);
        void Delete(int id);
    }
}
