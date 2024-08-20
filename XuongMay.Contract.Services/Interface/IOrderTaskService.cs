using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.OrderTaskModelViews;
using XuongMay.ModelViews.ProductTaskModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IOrderTaskService
    {
        // R
        Task<BasePaginatedList<OrderTask>> GetAllOrderTask(int index, int pageSize);
        Task<BasePaginatedList<OrderTask>> GetAllOrderTaskByFiler(string keyword, int index, int pageSize);

        // CUD
        Task InsertOrderTask(OrderTaskRequestModel obj);
        Task UpdateOrderTask(OrderTaskUpdateModel obj);
        Task UpdateOrderTaskStatus(OrderTaskUpdateModel obj);
        Task UpdateOrderTaskCompleteQuantity(OrderTaskUpdateCompleteQuantity obj);
        Task DeleteOrderTask(object orderTaskId, string deleteBy);
    }
}
