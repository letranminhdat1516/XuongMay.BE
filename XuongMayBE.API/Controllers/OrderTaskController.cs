using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.ModelViews.OrderTaskModelViews;
using XuongMay.ModelViews.ProductTaskModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTaskController : ControllerBase
    {
        private readonly IOrderTaskService _orderTaskService;

        public OrderTaskController(IOrderTaskService orderTaskService)
        {
            _orderTaskService = orderTaskService;
        }

        #region Lấy danh sách các nhiệm vụ
        [HttpGet()]
        [SwaggerOperation(Summary = "Lấy danh sách nhiệm vụ có phân trang")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> GetAllOrderTask(int index = 1, int pageSize = 5)
        {
            var orderTasks = await _orderTaskService.GetAllOrderTask(index, pageSize);
            return Ok(BaseResponse<BasePaginatedList<OrderTask>>.OkResponse(orderTasks));
        }
        #endregion

        #region Lấy danh sách nhiệm vụ theo filter
        [HttpGet("filter")]
        [SwaggerOperation(Summary = "Lấy danh sách nhiệm vụ theo filter")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> GetAllOrderTaskByFilter(string keyword = "", int index = 1, int pageSize = 10)
        {
            return Ok(await _orderTaskService.GetAllOrderTaskByFiler(keyword, index, pageSize));
        }
        #endregion

        #region Thêm mới nhiệm vụ cho băng chuyền
        [HttpPost()]
        [SwaggerOperation(Summary = "Thêm mới nhiệm vụ cho băng chuyền")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> InsertTask(OrderTaskRequestModel request)
        {
            try
            {
                request.CreateBy = "KietPHG";
                await _orderTaskService.InsertOrderTask(request);
                return Ok(BaseResponse<string>.OkResponse("Tạo mới nhiệm vụ thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return NotFound(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Cập nhật nhiệm vụ của băng chuyền
        [HttpPut()]
        [SwaggerOperation(Summary = "Cập nhật nhiệm vụ của băng chuyền")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> UpdateTask([FromBody] OrderTaskUpdateModel request)
        {
            try
            {
                request.UpdateBy = "KietPHG";
                await _orderTaskService.UpdateOrderTask(request);
                return Ok(BaseResponse<string>.OkResponse("Cập nhật nhiệm vụ thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Cập nhật trạng thái nhiệm vụ hoàn thành
        [HttpPut("{id}/TaskComplete")]
        [SwaggerOperation(Summary = "Cập nhật nhiệm vụ hoàn thành")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> UpdateTaskComplete(string id)
        {
            try
            {
                var orderTaskUpdate = new OrderTaskUpdateModel { OrderTaskId = id, UpdateBy = "KietPHG" };
                orderTaskUpdate.SetStatus("Completed");

                await _orderTaskService.UpdateOrderTaskStatus(orderTaskUpdate);

                return Ok(BaseResponse<string>.OkResponse("Cập nhật trạng thái thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Cập nhật trạng thái hủy nhiệm vụ
        [HttpPut("{id}/TaskCancel")]
        [SwaggerOperation(Summary = "Cập nhật hủy nhiệm vụ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> UpdateTaskStop(string id)
        {
            try
            {
                var orderTaskUpdate = new OrderTaskUpdateModel { OrderTaskId = id, UpdateBy = "KietPHG" };
                orderTaskUpdate.SetStatus("Canceled");

                await _orderTaskService.UpdateOrderTaskStatus(orderTaskUpdate);

                return Ok(BaseResponse<string>.OkResponse("Cập nhật trạng thái thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Cập nhật số lượng hoàn thành
        [HttpPut("{id}/CompleteQuantity")]
        [SwaggerOperation(Summary = "Cập nhật số lượng đơn hàng hoàn thành")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> UpdateCompleteQuantity(string id, int quantity)
        {
            try
            {
                var completeQuantity = new OrderTaskUpdateCompleteQuantity
                {
                    OrderTaskId = id,
                    Quantity = quantity,
                    UpdateBy = "KietPHG"
                };
                await _orderTaskService.UpdateOrderTaskCompleteQuantity(completeQuantity);
                return Ok(BaseResponse<string>.OkResponse("Cập nhật thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Cập nhật trạng thái tiếp tục nhiệm vụ
        //[HttpPut("TaskContinue/{id}")]
        //[SwaggerOperation(Summary = "Cập nhật nhiệm vụ tiếp tục")]
        //public async Task<IActionResult> UpdateTaskContinue(string id)
        //{
        //    try
        //    {
        //        var orderTaskUpdate = new OrderTaskUpdateModel { OrderTaskId = id, UpdateBy = "KietPHG" };
        //        orderTaskUpdate.SetStatus("Processing");

        //        await _orderTaskService.UpdateOrderTaskStatus(orderTaskUpdate);

        //        var response = BaseResponse<string>.OkResponse("Cập nhật trạng thái thành công");
        //        return Ok(response);
        //    }
        //    catch (BaseException.ErrorException ex)
        //    {
        //        var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
        //        return BadRequest(response);
        //    }
        //}
        #endregion

        #region Xóa nhiệm vụ của băng chuyền
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa nhiệm vụ của băng chuyền")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            try
            {
                await _orderTaskService.DeleteOrderTask(id, "KietPHG");
                return Ok(BaseResponse<string>.OkResponse("Xóa vụ thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion
    }
}
