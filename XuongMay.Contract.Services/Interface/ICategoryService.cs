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
        // interface CRUD of category
        Task<IList<Category>> GetAll();
        Task<Category> GetCategoryById(object id);
        Task<bool> CreateCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategoryById(object id);
    }
}
