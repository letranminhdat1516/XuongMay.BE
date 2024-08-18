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

namespace XuongMay.Services.Service
{
    public class OrderProductService : IOrderProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Orders> _orderRepository;
        public OrderProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository= _unitOfWork.GetRepository<Orders>();
        }
        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
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

        public Task InsertAsync(Orders obj)
        {
            throw new NotImplementedException();
        }

        public void InsertRange(IList<Orders> obj)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Orders obj)
        {
            throw new NotImplementedException();
        }
    }
}
