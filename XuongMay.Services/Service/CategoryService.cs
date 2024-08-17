using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;

namespace XuongMay.Services.Service
{

    public class CategoryService : ICategoryService
    {
        public Task CreateCategory(ProductTask productTask)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCategoryById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Category>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetCategoryById(object id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
