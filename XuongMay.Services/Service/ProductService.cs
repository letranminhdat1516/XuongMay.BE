using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //insert product
        public async Task CreateProduct(ProductModel products, ClaimsPrincipal userClaims)
        {
                Products productsTemp = new Products();
                productsTemp.ProductName = products.ProductName;
                productsTemp.ProductDescription = products.ProductDescription;
                productsTemp.ProductPrice = products.ProductPrice;
                productsTemp.CategoryId = products.CategoryId;
                productsTemp.CreatedBy = userClaims.Identity?.Name;
                productsTemp.CreatedTime = CoreHelper.SystemTimeNow;
                productsTemp.LastUpdatedTime = CoreHelper.SystemTimeNow;
            //Check category is exits

            bool hasCategoy = await _unitOfWork.GetRepository<Category>().Entities
                                     .AnyAsync(c => c.Id.Equals(productsTemp.CategoryId));
                if (!hasCategoy)
                {
                    throw new BaseException.BadRequestException("Bad Request", "Category does not exist");
                }
                await _unitOfWork.GetRepository<Products>().InsertAsync(productsTemp);
                await _unitOfWork.GetRepository<Products>().SaveAsync();
        }

        //remove product
        public async Task DeleteProductById(string id)
        {
            Products productsTemp = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if (productsTemp == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Product.");
            }
            //check for deleted products
            if (!productsTemp.IsDelete)
            {
                throw new BaseException.BadRequestException("Bad Request", "Active products cannot be deleted!!!");
            }
            await _unitOfWork.GetRepository<Products>().DeleteAsync(id);
            await _unitOfWork.GetRepository<Products>().SaveAsync();

        }

        //remove product by way update status
        public async Task DeleteProductByUpdateStatus(string id, ClaimsPrincipal userClaim)
        {
            Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if (product == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Product");
            }
            //check for deleted products

            if (product.IsDelete)
            {
                throw new BaseException.BadRequestException("Bad Request", "Product has been deleted ");
            }
            //Check the product included in the orders
            bool hasProducts = await _unitOfWork.GetRepository<Orders>().Entities
                         .AnyAsync(o => o.ProductId.Equals(product.Id));
            if (hasProducts)
            {
                throw new BaseException.BadRequestException("Bad Request", "Cannot delete active product !!!");
            }
            product.IsDelete = true;
            product.DeletedBy = userClaim.Identity?.Name;
            product.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Products>().UpdateAsync(product);
            await _unitOfWork.GetRepository<Products>().SaveAsync();
        }

        //get all product with paging
        public Task<BasePaginatedList<Products>> GetAllProductPaging(int index, int pageSize)
        {
            var productsTemp = _unitOfWork.GetRepository<Products>().Entities.Where(p => !p.IsDelete);
            var products = _unitOfWork.GetRepository<Products>().GetPagging(productsTemp, index, pageSize);
            return products;
        }

        //get product with filter
        public Task<BasePaginatedList<Products>> GetProductByFilter(string keyWord, int index, int pageSize)
        {
            var productsTemp = _unitOfWork.GetRepository<Products>().Entities.Where(p => !p.IsDelete && (p.ProductName.Contains(keyWord) || keyWord == string.Empty));
            var products = _unitOfWork.GetRepository<Products>().GetPagging(productsTemp, index, pageSize);
            return products;
        }

        //get product by id
        public async Task<Products> GetProdutcById(string id)
        {
            Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            return product;
        }

        //update product
        public async Task UpdateProduct(string id, ProductModel products, ClaimsPrincipal userClaim)
        {
            Products productTemp = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if (productTemp == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Product.");
            }
            //Check category is exits
            bool hasCategoy = await _unitOfWork.GetRepository<Category>().Entities
                                     .AnyAsync(c => c.Id.Equals(products.CategoryId));
            if (!hasCategoy)
            {
                throw new BaseException.BadRequestException("Bad Request", "Category does not exist");
            }
            productTemp.ProductName = products.ProductName;
            productTemp.ProductDescription = products.ProductDescription;
            productTemp.ProductPrice = products.ProductPrice;
            productTemp.CategoryId = products.CategoryId;
            productTemp.LastUpdatedBy = userClaim.Identity?.Name;
            productTemp.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Products>().UpdateAsync(productTemp);
            await _unitOfWork.GetRepository<Products>().SaveAsync();
        }

        //update status Isworking of product
        //public async Task UpdateProductStatus(string id, bool status, ClaimsPrincipal userClaim)
        //{
        //    Products productTemp = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
        //    if (productTemp == null)
        //    {
        //        throw new BaseException.ErrorException(404, "Not Found", "Not Found Product.");
        //    }
        //    productTemp.LastUpdatedBy = userClaim?.Identity?.Name;
        //    productTemp.LastUpdatedTime = CoreHelper.SystemTimeNow;
        //    await _unitOfWork.GetRepository<Products>().UpdateAsync(productTemp);
        //    await _unitOfWork.GetRepository<Products>().SaveAsync();
        //}
    }
}





