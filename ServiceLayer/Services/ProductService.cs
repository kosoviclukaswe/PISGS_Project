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
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task Create(Product product)
        {
            await _productRepository.Create(product);
        }

        public void Delete(int id)
        {
            _productRepository.Delete(id);
        }

        public void Delete(string name)
        {
            _productRepository.Delete(name);
        }

        public Product Read(int id)
        {
            return _productRepository.Read(id);
        }

        public Product Read(string name)
        {
            return _productRepository.Read(name);
        }

        public List<Product> ReadAll()
        {
            return _productRepository.ReadAll();
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }
    }
}
