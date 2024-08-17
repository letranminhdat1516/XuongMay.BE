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
        public async Task<bool> CreateCategory(Category category)
        {
            try
            {
                await _unitOfWork.GetRepository<Category>().InsertAsync(category);
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
        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
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
