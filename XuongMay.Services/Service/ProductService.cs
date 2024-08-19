using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public async Task<bool> CreateProduct(ProductModel products)
        {
            try
            {
                Products productsTemp = new Products();
                productsTemp.ProductName = products.ProductName;
                productsTemp.ProductDescription = products.ProductDescription;
                productsTemp.ProductPrice = products.ProductPrice;
                productsTemp.CreatedTime = CoreHelper.SystemTimeNow;
                productsTemp.LastUpdatedTime = CoreHelper.SystemTimeNow;
                await _unitOfWork.GetRepository<Products>().InsertAsync(productsTemp);
                await _unitOfWork.GetRepository<Products>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //remove product
        public async Task DeleteProductById(string id)
        {
            Products productsTemp = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if (productsTemp == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Product.");
            }
            await _unitOfWork.GetRepository<Products>().DeleteAsync(id);
            await _unitOfWork.GetRepository<Products>().SaveAsync();

        }

        //remove product by way update status
        public async Task DeleteProductByUpdateStatus(string id)
        {
            Products products = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if(products == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Product");
            }

            if(products.IsDelete)
            {
                throw new BaseException.BadRequestException("Bad Request", "Cannot delete active products");
            }
            products.IsDelete = true;
            products.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Products>().UpdateAsync(products);
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
        public async Task UpdateProduct(string id, ProductModel products)
        {
            Products productTemp = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if (products == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Product.");
            }
            productTemp.ProductName = products.ProductName;
            productTemp.ProductDescription = products.ProductDescription;
            productTemp.ProductPrice = products.ProductPrice;
            productTemp.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Products>().UpdateAsync(productTemp);
            await _unitOfWork.GetRepository<Products>().SaveAsync();
        }
    }
}





