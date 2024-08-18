using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.CategoryModelViews;

namespace XuongMay.Services.Service
{

    public class CategoryService : ICategoryService
    {
        //implement method of class ICategory
        private readonly IUnitOfWork _unitOfWork;
        
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //get all category
        public async Task<IList<CategoryModel>> GetAll()
        {
            IList<CategoryModel> categories = await _unitOfWork.GetRepository<CategoryModel>().GetAllAsync();
            return categories;
        }

        //get category by id
        public async Task<CategoryModel> GetCategoryById(string id)
        {
            CategoryModel category = await _unitOfWork.GetRepository<CategoryModel>().GetByIdAsync(id);
            return category;
        }

        //insert category
        public async Task<bool> CreateCategory(CategoryModel category)
        {
            try
            {
                await _unitOfWork.GetRepository<CategoryModel>().InsertAsync(category);
                await _unitOfWork.GetRepository<CategoryModel>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        //remove category
        public async Task<bool> DeleteCategoryById(string id)
        {
            try
            {
                await _unitOfWork.GetRepository<CategoryModel>().DeleteAsync(id);
                await _unitOfWork.GetRepository<CategoryModel>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        //update catogory
        public async Task<bool> UpdateCategory(CategoryModel category)
        {
            try
            {
                await _unitOfWork.GetRepository<CategoryModel>().UpdateAsync(category);
                await _unitOfWork.GetRepository<CategoryModel>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
