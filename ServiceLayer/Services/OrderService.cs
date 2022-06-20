using AutoMapper;
using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using ServiceLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository ingridientRepository, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = ingridientRepository;
        }

        public async Task Create(Order order)
        {
            await _orderRepository.Create(order);
        }

        public void Delete(int id)
        {
            _orderRepository.Delete(id);
        }

        public Order Read(int id)
        {
            return _orderRepository.Read(id);
        }

        public List<Order> ReadAll()
        {
            return _orderRepository.ReadAll();
        }

        public List<Order> ReadAll(string userId)
        {
            return _orderRepository.ReadAll(userId);
        }

        public List<Order> ReadAll(OrderState state)
        {
            return _orderRepository.ReadAll(state);
        }

        public List<Order> ReadAllForDeliverer(string delivererId)
        {
            return _orderRepository.ReadAll(delivererId);
        }

        public int DelivererHasActiveOrder(string delivererId)
        {
            var delivererOrders = _orderRepository.ReadAllForDeliverer(delivererId);
            var order = delivererOrders.Find(x => x.OrderState == OrderState.ACTIVE);
            if (order != null)
            {
                return order.OrderId;
            }
            return -1;
        }

        public List<Order> ReadCompletedOrdersByDeliverer(string delivererId)
        {
            var delivererOrders = _orderRepository.ReadAllForDeliverer(delivererId);
            var completed = delivererOrders.FindAll(x => x.OrderState == OrderState.COMPLETE);
            if (completed != null)
            {
                return completed.ToList();
            }
            return completed;
        }

        public void Update(Order order)
        {
            _orderRepository.Update(order);
        }
    }
}
