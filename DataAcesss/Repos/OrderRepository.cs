using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PUSGS_ProjectContext _context;
        public OrderRepository(PUSGS_ProjectContext context)
        {
            _context = context;
        }
        public async Task Create(Order order)
        {
            await _context.Orders.AddAsync(order);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public Order Read(int id)
        {
            return _context.Orders.FirstOrDefault(x => x.OrderId == id);
        }

        public List<Order> ReadAll()
        {
            return _context.Orders.ToList();
        }

        public List<Order> ReadAll(string userId)
        {
            var userOrders = _context.Orders.Where(x => x.AppUserId == userId);
            if (userOrders != null)
            {
                return userOrders.ToList();
            }
            return new List<Order>();
        }

        public List<Order> ReadAll(OrderState state)
        {
            var userOrders = _context.Orders.Where(x => x.OrderState == state);
            if (userOrders != null)
            {
                return userOrders.ToList();
            }
            return new List<Order>();
        }

        public List<Order> ReadAllForDeliverer(string delivererId)
        {
            var userOrders = _context.Orders.Where(x => x.DelivererId == delivererId);
            if (userOrders != null)
            {
                return userOrders.ToList();
            }
            return new List<Order>();
        }

        public void Update(Order order)
        {
            var newOrder = _context.Orders.FirstOrDefault(x => x.OrderId == order.OrderId);
            if (newOrder != null)
            {
                newOrder.Address = order.Address;
                newOrder.Comment = order.Comment;
                newOrder.OrderState = order.OrderState;
                newOrder.Price = order.Price;
                newOrder.Time = order.Time;
                newOrder.ProductOrders = order.ProductOrders;

                _context.Orders.Update(newOrder);
                _context.SaveChanges();
            }
        }
    }
}
