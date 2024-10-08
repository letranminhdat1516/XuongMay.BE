﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Core;
using XuongMay.ModelViews.OrderModelViews;
namespace XuongMay.Contract.Services.Interface
{
    public interface IOrderProductService
    {       
        //R get all the order from DB
        public Task<BasePaginatedList<Orders>> GetAllAsync(int index, int pageSize);
        //R get order with a specific id
        public Task<Orders?> GetByIdAsync(string id);
        //CUD
        public Task CreateOrder(OrderModelView order,string userName);
        public Task UpdateAsync(string id,OrderModelView obj, string userName);
        public Task DeleteAsync(string id,string name);
    }
}
