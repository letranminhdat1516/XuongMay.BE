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
        public async Task<IList<Category>> GetAll()
        {
            IList<Category> categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
            return categories;
        }

        //get category by id
        public async Task<Category> GetCategoryById(string id)
        {
            Category category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            return category;
        }

        //insert category
        public async Task<bool> CreateCategory(CategoryModel category)
        {
            try
            {
                Category categoryTemp = new Category();
                categoryTemp.CategoryName = category.CategoryName;
                categoryTemp.CategoryDescription = category.CategoryDescription;
                categoryTemp.CreatedTime = DateTimeOffset.UtcNow;
                categoryTemp.LastUpdatedTime = DateTimeOffset.UtcNow;
                await _unitOfWork.GetRepository<Category>().InsertAsync(categoryTemp);
                await _unitOfWork.GetRepository<Category>().SaveAsync();
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
                await _unitOfWork.GetRepository<Category>().DeleteAsync(id);
                await _unitOfWork.GetRepository<Category>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        //update catogory
        public async Task<bool> UpdateCategory(string id, CategoryModel category)
        {
            try
            {

                Category categoryTemp = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
                if(categoryTemp == null){
                    return false;
                }
                categoryTemp.CategoryName = category.CategoryName;
                categoryTemp.CategoryDescription = category.CategoryDescription;
                categoryTemp.LastUpdatedTime = DateTimeOffset.UtcNow;
                await _unitOfWork.GetRepository<Category>().UpdateAsync(categoryTemp);
                await _unitOfWork.GetRepository<Category>().SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
