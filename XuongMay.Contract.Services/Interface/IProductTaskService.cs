using XuongMay.Contract.Repositories.Entity;

namespace XuongMay.Contract.Services.Interface
{
    public interface IProductTaskService
    {
        // R
        Task<IList<ProductTask>> GetAll();
        Task<ProductTask?> GetTaskOfProductById(object id);

        // CUD
        Task InsertNewTask(ProductTask productTask);
        Task UpdateQuantityToDo(int quantity);
        Task DeleteTask(object id);
    }
}
