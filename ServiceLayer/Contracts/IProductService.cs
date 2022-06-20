using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Contracts
{
    public interface IProductService
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
