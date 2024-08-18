using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.Services.Service;
using XuongMay.ModelViews.CategoryModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //api get all category
        [HttpGet("get-all-category")]
        public async Task<IActionResult> GetAllCategory()
        {
            IList<Category> categories = await _categoryService.GetAll();
            if (categories == null)
            {
                return BadRequest();
            }
            return Ok(BaseResponse<IList<Category>>.OkResponse(categories));
        }

        //api get categoryby id
        [HttpGet("get-by-category-id/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            Category category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return BadRequest();
            }
            return Ok(BaseResponse<Category>.OkResponse(category));
        }

        //api insert category
        [HttpPost("insert-category")]
        public async Task<IActionResult> InsertCategory([FromBody] CategoryModel category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null.");
            }

            try
            {
                bool result = await _categoryService.CreateCategory(category);
                if (result)
                {
                    return Ok("Category inserted successfully.");
                }
                return BadRequest("Category inserted Fail !!!");
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        //api update category
        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryModel category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null");
            }
            try
            {
                bool result = await _categoryService.UpdateCategory(id, category);
                if (result)
                {
                    return Ok("Category update successfully.");
                }
                return BadRequest("Category update fail !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        //api remove category
        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                bool result = await _categoryService.DeleteCategoryById(id);
                if (result)
                {
                    return Ok("Category delete successfully.");
                }
                return BadRequest("Category delete fail !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
