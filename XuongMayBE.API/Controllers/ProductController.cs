using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;

namespace XuongMayBE.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        //api get all product
        [HttpGet("get-all-produtc")]
        public async Task<IActionResult> GetAllProduct()
        {
            IList<Products> products = await _productService.GetAll();
            if (products == null || products.Count == 0)
            {
                return BadRequest("List product empty");
            }
            return Ok(BaseResponse<IList<Products>>.OkResponse(products));
        }

        //api get product by id
        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            Products product = await _productService.GetProdutcById(id);
            if (product == null)
            {
                return BadRequest("Product does not exits !!!");
            }
            return Ok(BaseResponse<Products>.OkResponse(product));
        }

        //api insert product
        [HttpPost("insert-product")]
        public async Task<IActionResult> InsertProduct([FromBody] Products product)
        {
            if (product == null)
            {
                return BadRequest("Product can not empty!!!");
            }
            try
            {
                bool result = await _productService.CreateProduct(product);
                if (result)
                {
                    return Ok("Insert product successfully.");
                }
                return BadRequest("Insert product fail !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        //api update product
        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct([FromBody] Products product)
        {
            if (product == null)
            {
                return BadRequest("Product can not empty!!!");
            }
            try
            {
                bool result = await _productService.UpdateProduct(product);
                if (result)
                {
                    return Ok("Update product successfull.");
                }
                return BadRequest("Update produtc fail !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        //api delete product
        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                bool result = await _productService.DeleteProductById(id);
                if (result)
                {
                    return Ok("Delete product successfull.");
                }
                return BadRequest("Delete product fail !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }



    }
}
