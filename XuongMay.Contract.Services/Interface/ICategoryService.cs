﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;

namespace XuongMay.Contract.Services.Interface
{
    public interface ICategoryService
    {
        // R
        Task<IList<Category>> GetAll();
        Task<Category> GetCategoryById(object id);

        // CUD
        Task CreateCategory(ProductTask productTask);
        Task UpdateCategory(Category category);
        Task<bool> DeleteCategoryById(object id);
    }
}
