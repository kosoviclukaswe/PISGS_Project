using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos.Contracts
{
    public interface IOrderRepository
    {
        Task Create(Order order);
        Order Read(int id);
        List<Order> ReadAll();
        List<Order> ReadAll(string userId);
        List<Order> ReadAllForDeliverer(string delivererId);
        List<Order> ReadAll(OrderState state);
        void Update(Order order);
        void Delete(int id);
    }
}
