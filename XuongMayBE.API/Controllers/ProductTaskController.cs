using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTaskController : ControllerBase
    {
        private readonly IProductTaskService _productTaskService;
        public ProductTaskController(IProductTaskService productTaskService)
        {
            _productTaskService = productTaskService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllProductTask()
        {
            IList<ProductTask> productTasks = await _productTaskService.GetAll();
            return Ok(BaseResponse<IList<ProductTask>>.OkResponse(productTasks));
        }

        [HttpGet("Create")]
        public async Task<IActionResult> CreateTask()
        {
            return Ok();
        }
    }
}
