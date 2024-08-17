using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;

namespace XuongMay.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        //insert product
        public async Task<bool> CreateProduct(Products products)
        {
            try
            {
                await _unitOfWork.GetRepository<Products>().InsertAsync(products);
                await _unitOfWork.GetRepository<Products>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //remove product
        public async Task<bool> DeleteProductById(string id)
        {
            try
            {
                await _unitOfWork.GetRepository<Products>().DeleteAsync(id);
                await _unitOfWork.GetRepository<Products>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //get all product
        public async Task<IList<Products>> GetAll()
        {
            IList<Products> products = await _unitOfWork.GetRepository<Products>().GetAllAsync();
            return products;
        }

        //get product by id
        public async Task<Products> GetProdutcById(string id)
        {
            Products products = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            return products;
        }

        //update product
        public async Task<bool> UpdateProduct(Products products)
        {
            try
            {
                await _unitOfWork.GetRepository<Products>().UpdateAsync(products);
                await _unitOfWork.GetRepository<Products>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
