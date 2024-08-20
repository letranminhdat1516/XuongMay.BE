using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.ModelViews.OrderModelViews;
using XuongMay.Core.Utils;
using XuongMay.Core.Base;
using Microsoft.AspNetCore;


namespace XuongMay.Services.Service
{
    public class OrderProductService : IOrderProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Orders> _orderRepository;        

        public OrderProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = _unitOfWork.GetRepository<Orders>();
        }
        #region
        public Task<BasePaginatedList<Orders>> GetAllAsync(int index, int pageSize)
        {
            var orderList = _orderRepository.Entities.Where(order => !order.IsDelete);
            return _orderRepository.GetPagging(orderList, index, pageSize);
        }
        #endregion
        #region Get an order by Id
        public async Task<Orders?> GetByIdAsync(string id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }
        #endregion
        #region Create order
        public async Task CreateOrder(OrderModelView order, string userName)
        {            
            Orders _order = new Orders
            {                
                ProductId = order.ProductId,
                OrdersCode = order.OrdersCode,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                CreatedBy = userName,
                CreatedTime=CoreHelper.SystemTimeNow
            };
            await _orderRepository.InsertAsync(_order);
            await _orderRepository.SaveAsync();
        }
        #endregion

        #region Update order
        public async Task UpdateAsync(string orderId,OrderModelView order,string userName)
        {
            Orders _order = await _orderRepository.GetByIdAsync(orderId);
            if(_order != null) 
            {
                await _orderRepository.UpdateAsync(_order);                
                _order.ProductId = order.ProductId;
                _order.OrdersCode = order.OrdersCode;
                _order.Quantity = order.Quantity;
                _order.TotalPrice = order.TotalPrice;
                _order.LastUpdatedBy=userName;
                await _orderRepository.SaveAsync();
            }
            else
            {
                throw new BaseException.ErrorException(404, "Not found", "Order is not exited");
            }
        }
        #endregion

        #region Delete order
        public async Task DeleteAsync(string id, string name)
        {
            Orders _order = await _orderRepository.GetByIdAsync(id);
            if (_order != null)
            {
                await _orderRepository.UpdateAsync(_order);
                _order.IsDelete = true;
                _order.DeletedBy = name;
                _order.DeletedTime = CoreHelper.SystemTimeNow;
                await _orderRepository.SaveAsync();
            }
            else
            {
                throw new BaseException.ErrorException(404, "Not found", "Order is not exited");
            }
        }
        #endregion
    }
}
