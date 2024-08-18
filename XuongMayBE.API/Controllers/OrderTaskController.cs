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
        public async Task<IActionResult> GetAllTaskWithPaging(int index = 1, int pageSize = 5)
        {
            var orderTasks = await _orderTaskService.GetAllOrderTaskWithPaging(index, pageSize);
            var response = BaseResponse<BasePaginatedList<OrderTask>>.OkResponse(orderTasks);
            return Ok(response);
        }
        #endregion

        #region Thêm mới nhiệm vụ cho băng chuyền
        [HttpPost()]
        [SwaggerOperation(Summary = "Thêm mới nhiệm vụ cho băng chuyền")]
        public async Task<IActionResult> InsertTask(OrderTaskRequestModel request)
        {
            try
            {
                request.CreateBy = "KietPHG";
                await _orderTaskService.InsertOrderTask(request);
                var response = BaseResponse<string>.OkResponse("Tạo mới nhiệm vụ thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return NotFound(response);
            }
            catch (BaseException.BadRequestException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }
        #endregion

        #region Cập nhật nhiệm vụ của băng chuyền
        [HttpPut()]
        [SwaggerOperation(Summary = "Cập nhật nhiệm vụ của băng chuyền")]
        public async Task<IActionResult> UpdateTask([FromBody] OrderTaskUpdateModel request)
        {
            try
            {
                request.UpdateBy = "KietPHG";
                await _orderTaskService.UpdateOrderTask(request);
                var response = BaseResponse<string>.OkResponse("Cập nhật nhiệm vụ thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }
        #endregion

        #region Cập nhật trạng thái nhiệm vụ hoàn thành
        [HttpPut("TaskComplete/{id}")]
        [SwaggerOperation(Summary = "Cập nhật nhiệm vụ hoàn thành")]
        public async Task<IActionResult> UpdateTaskComplete(string id)
        {
            try
            {
                var orderTaskUpdate = new OrderTaskUpdateModel { OrderTaskId = id, UpdateBy = "KietPHG" };
                orderTaskUpdate.SetStatus("Completed");

                await _orderTaskService.UpdateOrderTaskStatus(orderTaskUpdate);

                var response = BaseResponse<string>.OkResponse("Cập nhật trạng thái thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }
        #endregion

        #region Cập nhật trạng thái hủy nhiệm vụ
        [HttpPut("TaskStop/{id}")]
        [SwaggerOperation(Summary = "Cập nhật hủy nhiệm vụ")]
        public async Task<IActionResult> UpdateTaskStop(string id)
        {
            try
            {
                var orderTaskUpdate = new OrderTaskUpdateModel { OrderTaskId = id, UpdateBy = "KietPHG" };
                orderTaskUpdate.SetStatus("Canceled");

                await _orderTaskService.UpdateOrderTaskStatus(orderTaskUpdate);

                var response = BaseResponse<string>.OkResponse("Cập nhật trạng thái thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
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
        public async Task<IActionResult> DeleteTask(string id)
        {
            try
            {
                await _orderTaskService.DeleteOrderTask(id, "KietPHG");
                var response = BaseResponse<string>.OkResponse("Xóa vụ thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }
        }
        #endregion
    }
}
