using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IList<UserResponseModel>> GetAll()
        {
            IList<UserResponseModel> users = new List<UserResponseModel>
            {
                new UserResponseModel { Id = "1" },
                new UserResponseModel { Id = "2" },
                new UserResponseModel { Id = "3" },
                new UserResponseModel { Id = "4" },
                new UserResponseModel { Id = "5" },
                new UserResponseModel { Id = "6" },
                new UserResponseModel { Id = "7" },
                new UserResponseModel { Id = "8" },
                new UserResponseModel { Id = "9" },
                new UserResponseModel { Id = "10" },
                new UserResponseModel { Id = "12" }
            };

            return Task.FromResult(users);
        }
    }
}
