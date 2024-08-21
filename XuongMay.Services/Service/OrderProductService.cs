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
        private readonly IGenericRepository<Products> _productRepository;

        public OrderProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = _unitOfWork.GetRepository<Orders>();
            _productRepository = _unitOfWork.GetRepository<Products>();
        }

        #region Get all Order
        public Task<BasePaginatedList<Orders>> GetAllAsync(int index, int pageSize)
        {
            var orderList = _orderRepository.Entities.Where(order => !order.IsDelete);
            return _orderRepository.GetPagging(orderList, index, pageSize);
        }
        #endregion

        #region Get an order by Id
        public async Task<Orders?> GetByIdAsync(string id)
        {
            Orders order = await _orderRepository.GetByIdAsync(id);
            if (!order.IsDelete)
            {
                return order;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Create order
        public async Task CreateOrder(OrderModelView order, string userName)
        {
            if (string.IsNullOrWhiteSpace(order.ProductId))
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Please fill product id");
            }
            var product = _productRepository.GetById(order?.ProductId);

            if (product == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not found the product");
            }

            Orders _order = new Orders
            {
                ProductId = order.ProductId,
                OrdersCode = order.OrdersCode,
                Quantity = order.Quantity,
                TotalPrice = order.Quantity * product.ProductPrice,
                CreatedBy = userName,
                CreatedTime = CoreHelper.SystemTimeNow
            };
            Task createOrder = _orderRepository.InsertAsync(_order);
            if (!createOrder.IsCompleted)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Create fail");
            }
            await _orderRepository.SaveAsync();
        }
        #endregion

        #region Update order
        public async Task UpdateAsync(string orderId, OrderModelView order, string userName)
        {
            if (string.IsNullOrWhiteSpace(order.ProductId))
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Please fill product id");
            }
            var product = _productRepository.GetById(order?.ProductId);

            if (product == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Not found the product");
            }

            Orders _order = await _orderRepository.GetByIdAsync(orderId);

            if (_order != null)
            {
                await _orderRepository.UpdateAsync(_order);
                _order.ProductId = order.ProductId;
                _order.OrdersCode = order.OrdersCode;
                _order.Quantity = order.Quantity;
                _order.TotalPrice = order.Quantity * product.ProductPrice;
                _order.LastUpdatedBy = userName;
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
