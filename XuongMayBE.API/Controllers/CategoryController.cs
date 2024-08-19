using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.Services.Service;
using XuongMay.ModelViews.CategoryModelViews;
using XuongMay.Core;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Azure.Core;

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
        public async Task<IActionResult> GetAllCategory(int index = 1, int pageSize = 9)
        {
            BasePaginatedList<Category> categories = await _categoryService.GetAllCategoryPaging(index, pageSize);
            if (categories == null)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse("List Category empty !!!"));
            }
            return Ok(BaseResponse<BasePaginatedList<Category>>.OkResponse(categories));
        }

        //api get category with filter
        [HttpGet("get-category-with-filter")]
        public async Task<IActionResult> GetCategoryWithFilter(string keyword = "", int index = 1, int pageSize = 9)
        {
            try
            {
                BasePaginatedList<Category> categories = await _categoryService.GetCategoryByFilter(keyword, index, pageSize);
                if (categories == null)
                {
                    return NotFound(BaseResponse<string>.NotFoundResponse("Not found category"));
                }
                return Ok(BaseResponse<BasePaginatedList<Category>>.OkResponse(categories));
            }
            catch (BaseException.ErrorException ex)
            {
                return NotFound(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        //api get category by id
        [HttpGet("get-category-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            try
            {
                Category category = await _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound(BaseResponse<string>.NotFoundResponse("Not found category"));
                }
                return Ok(BaseResponse<Category>.OkResponse(category));
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        //api insert category
        [HttpPost("insert-category")]
        public async Task<IActionResult> InsertCategory([FromBody] CategoryModel category)
        {
            try
            {
                await _categoryService.CreateCategory(category);
                var response = BaseResponse<string>.OkResponse("Category inserted successfully.");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }

        //api update category
        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryModel category)
        {
            try
            {
                await _categoryService.UpdateCategory(id, category);
                return Ok(BaseResponse<string>.OkResponse("Category update successfully."));
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }

        }

        //api remove category by way update status
        [HttpDelete("delete-category-by-update-status")]
        public async Task<IActionResult> DeleteCategoryByUpdateStatus(string id)
        {
            try
            {
                await _categoryService.DeleteCategoryByUpdateStatus(id);
                return Ok(BaseResponse<string>.OkResponse("Category delete successfully."));
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        //api remove category by id
        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                await _categoryService.DeleteCategoryById(id);
                return Ok(BaseResponse<string>.OkResponse("Category delete successfully."));
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
    }
}
