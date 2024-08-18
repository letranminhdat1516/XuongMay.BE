using Microsoft.Extensions.Configuration;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.UserModelViews;
using XuongMay.Repositories.Entity;
using XuongMayBE.API.Config;

namespace XuongMay.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<ApplicationRole> _roleRepo;
        private readonly IGenericRepository<ApplicationUserRoles> _userRoleRepo;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _userRoleRepo = _unitOfWork.GetRepository<ApplicationUserRoles>();
        }

        public async Task<IList<UserResponseModel>> GetAll()
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

            return await Task.FromResult(users);
        }

        public async Task<string?> RegisterAsync(RegisterModelView model)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUser>();

            // Kiểm tra xem người dùng đã tồn tại hay chưa
            var existingUser = userRepo.Entities.FirstOrDefault(u => u.UserName == model.Username);
            if (existingUser != null)
            {
                return "User already exists";
            }

            // Hash mật khẩu trước khi lưu vào cơ sở dữ liệu
            var hashedPassword = HashHelper.ComputeSha256Hash(model.Password);

            // Tạo người dùng mới 
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Password = model.Password,
                PasswordHash = hashedPassword,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };

            // Lưu người dùng mới vào cơ sở dữ liệu
            await userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            // Kiểm tra xem vai trò đã tồn tại chưa, nếu chưa thì thêm
            var role = _roleRepo.Entities.FirstOrDefault(r => r.Name == "ConveyorManager");
            if (role == null)
            {
                role = new ApplicationRole
                {
                    Name = "ConveyorManager",
                    NormalizedName = "CONVEYORMANAGER",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
                };
                await _roleRepo.InsertAsync(role);
                await _unitOfWork.SaveAsync();
            }

            // Gán vai trò cho người dùng
            var userRole = new ApplicationUserRoles
            {
                UserId = user.Id,
                RoleId = role.Id,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            await _userRoleRepo.InsertAsync(userRole);
            await _unitOfWork.SaveAsync();

            return null;
        }

        public async Task<string?> LoginAsync(LoginModelView model)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUser>();

            // Tìm người dùng trong cơ sở dữ liệu
            var user = userRepo.Entities.FirstOrDefault(u => u.UserName == model.Username);
            if (user == null)
            {
                return "Invalid credentials";
            }

            // Hash mật khẩu nhập vào và so sánh với mật khẩu lưu trong cơ sở dữ liệu
            var hashedPassword = HashHelper.ComputeSha256Hash(model.Password);
            if (user.PasswordHash != hashedPassword)
            {
                return "Invalid credentials";
            }

            // Lấy vai trò của người dùng từ cơ sở dữ liệu
            var roleId = _userRoleRepo.Entities
                .Where(r => r.UserId == user.Id)
                .Select(r => r.RoleId)
                .FirstOrDefault();

            if (roleId == default)
            {
                return "User role not found";
            }

            // Lấy tên role từ bảng ApplicationRole
            var role = _roleRepo.Entities
                .Where(r => r.Id == roleId)
                .Select(r => r.Name)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(role))
            {
                return "User role not found";
            }

            // Phát hành JWT token
            string token = TokenHelper.GenerateJwtToken(
                user,
                role,
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]);

            return token;
        }
    }
}