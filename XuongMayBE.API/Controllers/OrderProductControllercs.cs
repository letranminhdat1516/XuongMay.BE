using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.ModelViews.OrderModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderProductControllercs : ControllerBase
    {
        private readonly IOrderProductService _orderProductService;
        public OrderProductControllercs(IOrderProductService orderProductService)
        {
            _orderProductService = orderProductService;
        }
        //get all order
        [HttpGet("all-order")]
        public async Task<IActionResult> GetAllOrder()
        {
            IList<Orders> orders=await _orderProductService.GetAllAsync();
            return Ok(BaseResponse<IList<Orders>>.OkResponse(orders));
        }
        //get a specific order
        [HttpGet("/{id}")]
        public async Task<Orders?> GetOrderById([FromRoute] string id)
        {
            return await _orderProductService.GetByIdAsync(id);            
        }
        //create a new order
        [HttpPost("/orders")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModelView orderModelView)
        {
            Task createOrder = _orderProductService.CreateOrder(orderModelView);
            await createOrder;
            if (createOrder.IsCompleted == true)
            {
                return Ok(BaseResponse<Orders>.OkResponse("Create successfully!"));
            }
            else
            {
                return Ok(BaseResponse<Orders>.OkResponse("Create fail"));
            }   
        }
        //change an exited order
        [HttpPut("/update-order/{id}")]
        public async Task<IActionResult> UpdateOrder(string id, OrderModelView orderModelView)
        {
            Task updateOrder = _orderProductService.UpdateAsync(id,orderModelView);
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
        //delete an order
        [HttpDelete("/delete/{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            //Orders deletedOrder = await _orderProductService.GetByIdAsync(id);
            Task orderDeleted = _orderProductService.DeleteAsync(id);
            await orderDeleted;
            ;
            if(orderDeleted.IsCompleted == true) 
            {
                return Ok(BaseResponse<Orders>.OkResponse("Delete successfully!"));
            }
            else 
            {
                return Ok(BaseResponse<Orders>.OkResponse("Delete fail"));
            }
        }
    }            
}
