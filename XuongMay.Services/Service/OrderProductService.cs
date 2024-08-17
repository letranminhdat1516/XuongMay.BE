using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Repositories.UOW;
using XuongMay.ModelViews.OrderModelViews;
using XuongMay.Core.Base;
using XuongMay.Repositories.Context;

namespace XuongMay.Services.Service
{
    public class OrderProductService : BaseEntity,IOrderProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Orders> _orderRepository;

        public OrderProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = _unitOfWork.GetRepository<Orders>();
        }
        //R get all the order from DB
        public async Task<IList<Orders>> GetAllAsync()
        {
            IList<Orders> orderList = await _unitOfWork.GetRepository<Orders>().GetAllAsync();
            return orderList;
        }
        //R get order with a specific id
        public async Task<Orders?> GetByIdAsync(string id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }
        //Create order
        public async Task CreateOrder(OrderModelView order)
        {
            Orders _order = new Orders
            {                
                UserInfoId = order.UserInfoId,
                ProductId = order.ProductId,
                OrdersCode = order.OrdersCode,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
            };
            await _orderRepository.InsertAsync(_order);
            await _orderRepository.SaveAsync();
        }
        public async Task UpdateAsync(string id,OrderModelView order)
        {
            Orders _order = await _orderRepository.GetByIdAsync(id);
            if(_order != null) 
            {
                await _orderRepository.UpdateAsync(_order);                
                _order.UserInfoId = order.UserInfoId;
                _order.ProductId = order.ProductId;
                _order.OrdersCode = order.OrdersCode;
                _order.Quantity = order.Quantity;
                _order.TotalPrice = order.TotalPrice;
                await _orderRepository.SaveAsync();
            }                        
        }
        //delete an order with specific id
        public async Task DeleteAsync(string id)
        {
            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveAsync();
        }
    }
}
