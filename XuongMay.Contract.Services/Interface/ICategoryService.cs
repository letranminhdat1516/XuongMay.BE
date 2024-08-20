using System.Security.Claims;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.CategoryModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface ICategoryService
    {
        // interface CRUD of category
        //get all category with paging
        Task<BasePaginatedList<Category>> GetAllCategoryPaging(int index, int pagSize);
        //get category with filter
        Task<BasePaginatedList<Category>> GetCategoryByFilter(string keyWord, int index, int pageSize);
        //get category by id
        Task<Category> GetCategoryById(string id);
        //create category
        Task<bool> CreateCategory(CategoryModel category, ClaimsPrincipal usersClaims);
        //update category
        Task UpdateCategory(string id, CategoryModel category ,ClaimsPrincipal userClaims);
        //delete category by way update isDelete
        Task DeleteCategoryByUpdateStatus(string id, ClaimsPrincipal userClaims);
        //delete category by id
        Task DeleteCategoryById(string id);
    }
}
