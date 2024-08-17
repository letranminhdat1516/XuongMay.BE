using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;

namespace XuongMay.Contract.Services.Interface
{
    public interface IProductService
    {
        // interface CRUD of product
        Task<IList<Products>> GetAll();
        Task<Products> GetProdutcById(string id);
        Task<bool> CreateProduct(Products products);
        Task<bool> UpdateProduct(Products products);
        Task<bool> DeleteProductById(string id);
    }
}
