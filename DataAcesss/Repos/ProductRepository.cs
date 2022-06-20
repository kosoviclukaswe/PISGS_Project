using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos
{
    public class ProductRepository : IProductRepository
    {
        private readonly PUSGS_ProjectContext _context;
        public ProductRepository(PUSGS_ProjectContext context)
        {
            _context = context;
        }
        public async Task Create(Product product)
        {
            await _context.Products.AddAsync(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public void Delete(string name)
        {
            var product = _context.Products.FirstOrDefault(x => x.Name == name);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public Product Read(int id)
        {
            return _context.Products.FirstOrDefault(x => x.ProductId == id);
        }

        public Product Read(string name)
        {
            return _context.Products.FirstOrDefault(x => x.Name == name);
        }

        public List<Product> ReadAll()
        {
            return _context.Products.ToList();
        }

        public void Update(Product product)
        {
            var newProduct = _context.Products.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (newProduct != null)
            {
                newProduct.Ingridients = product.Ingridients;
                newProduct.Name = product.Name;
                newProduct.Price = product.Price;
                newProduct.ProductOrders = product.ProductOrders;

                _context.Products.Update(newProduct);
                _context.SaveChanges();
            }
        }
    }
}
