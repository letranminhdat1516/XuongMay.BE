using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;

namespace XuongMay.Services.Service
{
    public class ProductTaskService : IProductTaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductTaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // R
        public async Task DeleteTask(object id)
        {
            await _unitOfWork.GetRepository<ProductTask>().DeleteAsync(id);
            await _unitOfWork.GetRepository<ProductTask>().SaveAsync();
        }

        public async Task<IList<ProductTask>> GetAll()
        {
            IList<ProductTask> productTasks = await _unitOfWork.GetRepository<ProductTask>().GetAllAsync();
            return productTasks;
        }

        public Task<ProductTask?> GetTaskOfProductById(object id)
        {
            throw new NotImplementedException();
        }

        // CUD
        public async Task InsertNewTask(ProductTask productTask)
        {
            await _unitOfWork.GetRepository<ProductTask>().InsertAsync(productTask);
            await SaveAsync();
        }

        public Task UpdateQuantityToDo(int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await _unitOfWork.SaveAsync();
        }
    }
}
