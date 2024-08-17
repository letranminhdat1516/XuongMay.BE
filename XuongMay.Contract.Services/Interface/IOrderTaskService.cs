using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.ProductTaskModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IOrderTaskService
    {
        // R
        Task<IList<OrderTask>> GetAllOrderTask();
        Task<BasePaginatedList<OrderTask>> GetAllOrderTaskWithPaging(int index, int pageSize);
        Task<OrderTask?> GetOrderTaskById(object id);

        // CUD
        Task InsertOrderTask(OrderTask obj);
        Task UpdateOrderTask(string id, OrderTaskRequestModel obj);
        Task UpdateOrderTaskStatus(string id, OrderTaskRequestModel obj);
        Task DeleteOrderTask(object id, string deleteBy);
    }
}
