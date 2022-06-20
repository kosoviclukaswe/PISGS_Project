using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos.Contracts
{
    public interface IProductRepository
    {
        Task Create(Product product);
        Product Read(int id);
        Product Read(string name);
        List<Product> ReadAll();
        void Update(Product product);
        void Delete(int id);
        void Delete(string name);
    }
}
