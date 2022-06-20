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
    public class ProductOrderService : IProductOrderService
    {
        private readonly IMapper _mapper;
        private readonly IProductOrderRepository _productOrderRepository;

        public ProductOrderService(IMapper mapper, IProductOrderRepository productOrderRepository)
        {
            _mapper = mapper;
            _productOrderRepository = productOrderRepository;
        }
        public List<ProductOrder> ReadAll()
        {
            return _productOrderRepository.ReadAll();
        }

        public List<ProductOrder> ReadForOrder(int orderId)
        {
            return _productOrderRepository.ReadForOrder(orderId);
        }
    }
}
