using System.Security.Claims;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IProductService
    {
        // interface CRUD of product
        //get all product with paging
        Task<BasePaginatedList<Products>> GetAllProductPaging(int index, int pageSize);
        //get product with filter
        Task<BasePaginatedList<Products>> GetProductByFilter(string keyWord, int index, int pageSize);
        //get product by it
        Task<Products> GetProdutcById(string id);
        //insert product
        Task CreateProduct(ProductModel products, ClaimsPrincipal userClaims);
        //update product
        Task UpdateProduct(string id, ProductModel products, ClaimsPrincipal userClaims);
        //update status working of product
        //Task UpdateProductStatus(string id, bool status, ClaimsPrincipal userClaims);
        //delete product by way update status
        Task DeleteProductByUpdateStatus(string id, ClaimsPrincipal userClaims);
        //delete product by id
        Task DeleteProductById(string id);
    }
}
