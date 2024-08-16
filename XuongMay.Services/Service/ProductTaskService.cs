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
        public async Task<bool> CompareQuantityTaskOfOrderDetail(object orderDetailId)
        {
            // Compare that the Task Quantity has assign not > Order quantity
            return true;
        }

        public async Task<IList<ProductTask>> GetAll()
        {
            IList<ProductTask> productTasks = await _unitOfWork.GetRepository<ProductTask>().GetAllAsync();
            return productTasks;
        }
        public Task<ProductTask> GetTaskOfProductById(object id)
        {
            throw new NotImplementedException();
        }

        // CUD
        public async Task CreateTaskFromOrder(ProductTask productTask)
        {
            await _unitOfWork.GetRepository<ProductTask>().InsertAsync(productTask);
            await _unitOfWork.GetRepository<ProductTask>().SaveAsync();
        }

        public Task CreateTaskFromOrderDetail(ProductTask productTask)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompleteQuantity(int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
