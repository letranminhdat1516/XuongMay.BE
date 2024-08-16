using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.Services.Service;

namespace XuongMayBE.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService) {
            _categoryService = categoryService;
        }

        [HttpGet("getAllCategory")]
        public async Task<IActionResult> GetAllCategory() {
            IList<Category> categories = await _categoryService.GetAll();
            if(categories == null){
                return BadRequest();
            }
            return Ok(BaseResponse<IList<Category>>.OkResponse(categories));
        }

        [HttpGet("getByCategoryId/{id}")]
        public async Task<IActionResult> GetCategoryById(object id)
        {
            Category category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return BadRequest();
            }
            return Ok(BaseResponse<Category>.OkResponse(category));
        }
    }
}
