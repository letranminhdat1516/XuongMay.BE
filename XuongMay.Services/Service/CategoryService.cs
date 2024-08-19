using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
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

        //get all category with paging
        public Task<BasePaginatedList<Category>> GetAllCategoryPaging(int index, int pagSize)
        {
            var categoriesTemp = _unitOfWork.GetRepository<Category>().Entities.Where(ca => !ca.IsDelete);
            var categories = _unitOfWork.GetRepository<Category>().GetPagging(categoriesTemp, index, pagSize);
            return categories;
        }

        //get category with filter
        public Task<BasePaginatedList<Category>> GetCategoryByFilter(string keyWord, int index, int pageSize)
        {
            var categoriesTemp = _unitOfWork.GetRepository<Category>().Entities.Where(ca => !ca.IsDelete && (ca.CategoryName.Contains(keyWord) || keyWord == string.Empty));
            var categories = _unitOfWork.GetRepository<Category>().GetPagging(categoriesTemp, index, pageSize);
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

        //remove category by id
        public async Task DeleteCategoryById(string id)
        {
            Category? categoryTemp = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (categoryTemp == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Category");
            }
            if (categoryTemp.IsDelete)
            {
                throw new BaseException.BadRequestException("Bad Request", "Cannot delete active categories");
            }
            await _unitOfWork.GetRepository<Category>().DeleteAsync(categoryTemp.Id);
            await _unitOfWork.GetRepository<Category>().SaveAsync();
        }

        //delete category by way update isWorking
        public async Task DeleteCategoryByUpdateStatus(string id)
        {
            Category? categoryTemp = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (categoryTemp == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Category");
            }
            if (categoryTemp.IsDelete)
            {
                throw new BaseException.BadRequestException("Bad Request", "Cannot delete active categories");
            }
            categoryTemp.IsDelete = true;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(categoryTemp);
            await _unitOfWork.GetRepository<Category>().SaveAsync();
        }

        //update catogory
        public async Task UpdateCategory(string id, CategoryModel category)
        {

            Category? categoryTemp = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (categoryTemp == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not Found Category");
            }
            categoryTemp.CategoryName = category.CategoryName;
            categoryTemp.CategoryDescription = category.CategoryDescription;
            categoryTemp.LastUpdatedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(categoryTemp);
            await _unitOfWork.GetRepository<Category>().SaveAsync();

        }
    }
}
