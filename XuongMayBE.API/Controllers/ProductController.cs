using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, ConveyorManager")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        //api get all product
        [HttpGet("get-all-produtc")]
        [Authorize(Policy = "ViewPolicy")]
        public async Task<IActionResult> GetAllProduct(int index = 1, int pageSize = 9)
        {
            BasePaginatedList<Products> products = await _productService.GetAllProductPaging(index, pageSize);
            if (products == null)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse("List Product Empty !!!"));
            }
            return Ok(BaseResponse<BasePaginatedList<Products>>.OkResponse(products));
        }

        //api get product with filter
        [HttpGet("get-product-with-filter")]
        [Authorize(Policy = "ViewPolicy")]
        public async Task<IActionResult> GetProductWithFilter(string keyWord = "", int index = 1, int pageSize = 9)
        {
            try
            {
                BasePaginatedList<Products> products = await _productService.GetProductByFilter(keyWord, index, pageSize);
                if (products == null)
                {
                    return NotFound(BaseResponse<string>.NotFoundResponse("Not found category"));
                }
                return Ok(BaseResponse<BasePaginatedList<Products>>.OkResponse(products));
            }
            catch (BaseException.ErrorException ex)
            {
                return NotFound(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        //api get product by id
        [HttpGet("get-product-by-id/{id}")]
        [Authorize(Policy = "ViewPolicy")]
        public async Task<IActionResult> GetProduct(string id)
        {
            try
            {
                Products products = await _productService.GetProdutcById(id);
                if (products == null)
                {
                    return NotFound(BaseResponse<string>.NotFoundResponse("Not found product"));
                }
                return Ok(BaseResponse<Products>.OkResponse(products));
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

        //api insert product
        [HttpPost("insert-product")]
        [Authorize(Policy = "InsertPolicy")]
        public async Task<IActionResult> InsertProduct([FromBody] ProductModel product)
        {
            try
            {
                await _productService.CreateProduct(product,User);
                var response = BaseResponse<string>.OkResponse("Product inserted successfully.");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }

        //api update product
        [HttpPut("update-product/{id}")]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductModel product)
        {
            try
            {
                await _productService.UpdateProduct(id, product,User);
                return Ok(BaseResponse<string>.OkResponse("Product update successfully."));
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

        //api update status IsWorking of product
        //[HttpPut("update-status")]
        ////[Authorize(Roles = "Admin,Users")]
        //public async Task<IActionResult> UpdateStatus(string id, bool status)
        //{
        //    try
        //    {
        //        await _productService.UpdateProductStatus(id, status,User);
        //        return Ok(BaseResponse<string>.OkResponse("Update status of product successfully."));
        //    }
        //    catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
        //    {
        //        return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
        //    }
        //    catch (BaseException.BadRequestException ex)
        //    {
        //        return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
        //    }
        //}

        //api delete product
        [HttpDelete("delete-product/{id}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                await _productService.DeleteProductById(id);
                return Ok(BaseResponse<string>.OkResponse("Product delete successfully."));
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

        //api remove product by way update status
        [HttpDelete("delete-product-by-update-status")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteProductByUpdateStatus(string id)
        {
            try
            {
                await _productService.DeleteProductByUpdateStatus(id,User);
                return Ok(BaseResponse<string>.OkResponse("Product delete successfully."));
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
