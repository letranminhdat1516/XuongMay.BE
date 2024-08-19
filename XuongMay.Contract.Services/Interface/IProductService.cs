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
