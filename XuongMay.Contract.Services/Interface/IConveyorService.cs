using XuongMay.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.ConveyorModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IConveyorService
    {
        // R
        Task<BasePaginatedList<Conveyor>> GetAllConveyorPaging(int index, int pageSize);
        Task<BasePaginatedList<Conveyor>> GetConveyorByFilter(string keyword, int index, int pageSize);

        // CUD
        Task InsertNewConveyor(ConveyorRequestModel obj);
        Task UpdateConveyor(ConveyorUpdateModel obj);
        Task DeleteConveyor(object id, string deleteBy);
        Task SaveAsync();

    }
}
