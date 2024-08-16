using XuongMay.Contract.Repositories.Entity;

namespace XuongMay.Contract.Services.Interface
{
    public interface IProductTaskService
    {
        // R
        Task<IList<ProductTask>> GetAll();
        Task<ProductTask> GetTaskOfProductById(object id);

        // CUD
        Task CreateTaskFromOrderDetail(ProductTask productTask);
        Task UpdateCompleteQuantity(int quantity);
        Task<bool> CompareQuantityTaskOfOrderDetail(object orderDetailId);
    }
}
