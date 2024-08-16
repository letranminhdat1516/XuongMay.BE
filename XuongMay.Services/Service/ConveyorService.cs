using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.ModelViews.ConveyorModelViews;

namespace XuongMay.Services.Service
{
    public class ConveyorService : IConveyorService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ConveyorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // R
        public async Task<IList<Conveyor>> GetAllConveyor()
        {
            return await _unitOfWork.GetRepository<Conveyor>().GetAllAsync();
        }

        public Task<BasePaginatedList<Conveyor>> GetAllConveyorPaging(int index, int pageSize)
        {
            IEnumerable<Conveyor> conveyors = _unitOfWork.GetRepository<Conveyor>().GetAll();
            Task<BasePaginatedList<Conveyor>> conveyorsPaging = _unitOfWork.GetRepository<Conveyor>().GetPagging(conveyors.AsQueryable(), index, pageSize);
            return conveyorsPaging;
        }

        public Task<Conveyor?> GetOneConveyor(object id)
        {
            Conveyor? conveyor = _unitOfWork.GetRepository<Conveyor>().GetById(id);
            return Task.FromResult(conveyor);
        }

        // CUD
        public async Task InsertNewConveyor(Conveyor obj)
        {
            await _unitOfWork.GetRepository<Conveyor>().InsertAsync(obj);
            await SaveAsync();
        }

        public async Task UpdateConveyor(Conveyor obj)
        {
            await _unitOfWork.GetRepository<Conveyor>().UpdateAsync(obj);
            await SaveAsync();
        }
        public async Task DeleteConveyor(object id)
        {
            await _unitOfWork.GetRepository<Conveyor>().DeleteAsync(id);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _unitOfWork.SaveAsync();
        }
    }
}
