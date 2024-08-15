using XuongMay.Contract.Repositories.Entity;

namespace XuongMay.Contract.Services.Interface
{
    public interface IProductTaskService
    {
        Task<IList<ProductTask>> GetAll();
        Task CreateTaskFromOrder(int id, int quantity, ProductTask productTask);
    }
}
