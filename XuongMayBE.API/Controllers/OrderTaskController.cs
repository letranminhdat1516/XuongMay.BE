using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.ModelViews.ProductTaskModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTaskController : ControllerBase
    {
        private readonly IOrderTaskService _orderTaskService;

        public ProductTaskController(IOrderTaskService orderTaskService)
        {
            _orderTaskService = orderTaskService;
        }

        [HttpGet()]
        [SwaggerOperation(Summary = "Lấy danh sách nhiệm vụ có phân trang")]
        public async Task<IActionResult> GetAllTaskWithPaging(int index = 1, int pageSize = 5)
        {
            var orderTasks = await _orderTaskService.GetAllOrderTaskWithPaging(index, pageSize);
            var response = BaseResponse<BasePaginatedList<OrderTask>>.OkResponse(orderTasks);
            return Ok(response);
        }

        [HttpPost()]
        [SwaggerOperation(Summary = "Thêm mới nhiệm vụ cho băng chuyền")]
        public async Task<IActionResult> CreateTask(OrderTaskRequestModel request)
        {
            try
            {
                var orderTask = new OrderTask
                {
                    OrderId = request.OrderId,
                    ConveyorId = request.ConvenyorId,
                    Quantity = request.Quantity,
                    TaskNote = request.TaskNote,
                };
                await _orderTaskService.InsertOrderTask(orderTask);
                var response = BaseResponse<string>.OkResponse("Tạo mới nhiệm vụ thành công");
                return Ok(response);
            }
            catch (BaseException.BadRequestException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật nhiệm vụ cho băng chuyền")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] OrderTaskRequestModel request)
        {
            return Ok();
        }

        [HttpDelete()]
        [SwaggerOperation(Summary = "Xóa nhiệm vụ của băng chuyền")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            return Ok();
        }
    }
}
