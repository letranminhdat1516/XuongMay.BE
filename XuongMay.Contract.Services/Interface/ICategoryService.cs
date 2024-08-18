﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.ModelViews.CategoryModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface ICategoryService
    {
        // interface CRUD of category
        Task<IList<CategoryModel>> GetAll();
        Task<CategoryModel> GetCategoryById(string id);
        Task<bool> CreateCategory(CategoryModel category);
        Task<bool> UpdateCategory(CategoryModel category);
        Task<bool> DeleteCategoryById(string id);
    }
}
