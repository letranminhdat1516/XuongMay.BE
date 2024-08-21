using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.ModelViews.OrderModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
    public class OrderProductControllercs : ControllerBase
    {
        private readonly IOrderProductService _orderProductService;
        public OrderProductControllercs(IOrderProductService orderProductService)
        {
            _orderProductService = orderProductService;
        }

        #region Get all the order
        [HttpGet("all-order")]
        [SwaggerOperation(Summary = "Lấy tất cả order")]
        public async Task<IActionResult> GetAllOrder(int index = 1, int pageSize = 10)
        {
            BasePaginatedList<Orders> orders = await _orderProductService.GetAllAsync(index, pageSize);
            return Ok(BaseResponse<BasePaginatedList<Orders>>.OkResponse(orders));
        }
        #endregion

        #region Get a specific order base on ID
        [HttpGet("/{id}")]
        [SwaggerOperation(Summary = "Lấy order dựa theo orderId")]
        public async Task<Orders?> GetOrderById([FromRoute] string id)
        {
            return await _orderProductService.GetByIdAsync(id);
        }
        #endregion

        #region Create a new order
        [HttpPost("/orders")]
        [SwaggerOperation(Summary = "Tạo 1 order mới")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModelView orderModelView)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                Task createOrder = _orderProductService.CreateOrder(orderModelView, userName);
                await createOrder;
                return Ok(BaseResponse<Orders>.OkResponse("Create successfully!"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Change detail of an exting order
        [HttpPut("/update-order/{id}")]
        [SwaggerOperation(Summary = "Chỉnh sửa 1 order dựa theo Id")]
        public async Task<IActionResult> UpdateOrder(string id, OrderModelView orderModelView)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            Task updateOrder = _orderProductService.UpdateAsync(id, orderModelView, userName);
            await updateOrder;
            if (updateOrder.IsCompleted == true)
            {
                return Ok(BaseResponse<Orders>.OkResponse("Update successfully!"));
            }
            else
            {
                return Ok(BaseResponse<Orders>.OkResponse("U fail"));
            }
        }
        #endregion

        #region Delete an order(change the IsDelete to true)
        [HttpDelete("/delete/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [SwaggerOperation(Summary = "Xoá 1 order")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var deleterName = User.FindFirst(ClaimTypes.Name)?.Value;
            Task orderDeleted = _orderProductService.DeleteAsync(id, deleterName);
            await orderDeleted;
            ;
            if (orderDeleted.IsCompleted == true)
            {
                return Ok(BaseResponse<Orders>.OkResponse("Delete successfully!"));
            }
            else
            {
                return Ok(BaseResponse<Orders>.OkResponse("Delete fail"));
            }
        }
        #endregion
    }
}
