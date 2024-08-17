using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.ConveyorModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IConveyorService
    {
        // R
        Task<IList<Conveyor>> GetAllConveyor();
        Task<BasePaginatedList<Conveyor>> GetAllConveyorPaging(int index, int pageSize);
        Task<Conveyor?> GetOneConveyor(object id);


        // CUD
        Task InsertNewConveyor(Conveyor obj);
        Task UpdateConveyor(string id, ConveyorRequestModel obj);
        Task DeleteConveyor(object id, string deleteBy);
        Task SaveAsync();

    }
}
