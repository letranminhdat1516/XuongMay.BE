using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.OrderTaskModelViews;
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
        Task InsertOrderTask(OrderTaskRequestModel obj);
        Task UpdateOrderTask(OrderTaskUpdateModel obj);
        Task UpdateOrderTaskStatus(OrderTaskUpdateModel obj);
        Task DeleteOrderTask(object orderTaskId, string deleteBy);
    }
}
