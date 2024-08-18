using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.ModelViews.OrderModelViews;
using XuongMay.Repositories.UOW;
namespace XuongMay.Contract.Services.Interface
{
    public interface IOrderProductService
    {       
        //R get all the order from DB
        public Task<IList<Orders>> GetAllAsync();
        //R get order with a specific id
        public Task<Orders?> GetByIdAsync(string id);
        //CUD
        public Task CreateOrder(OrderModelView order);
        public Task UpdateAsync(string id,OrderModelView obj);
        public Task DeleteAsync(string id);
    }
}
