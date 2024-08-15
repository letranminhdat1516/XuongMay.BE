using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<IList<UserResponseModel>> GetAll();
    }
}
