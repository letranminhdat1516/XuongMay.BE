using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
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
        private readonly IGenericRepository<ApplicationUser> _userRepo;
        private readonly IGenericRepository<ApplicationRole> _roleRepo;
        private readonly IGenericRepository<ApplicationUserRoles> _userRoleRepo;
        private readonly IGenericRepository<UserInfo> _userInfoRepo;

        // Constructor injects necessary dependencies
        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _userRoleRepo = _unitOfWork.GetRepository<ApplicationUserRoles>();
            _userInfoRepo = _unitOfWork.GetRepository<UserInfo>();
        }
        // Retrieve users by id
        public async Task<UserResponseModel?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID không được rỗng.", nameof(userId));
            }

            var user = await _userRepo.Entities
                .Include(u => u.UserInfo)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId) && !u.UserInfo.IsDelete);

            if (user == null)
            {
                return null;
            }

            return new UserResponseModel
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.UserInfo?.FullName,
                PhoneNumber = user.PhoneNumber,
                CreatedTime = user.CreatedTime,
                LastUpdatedTime = user.LastUpdatedTime,
                IsDelete = user.UserInfo?.IsDelete ?? false,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed
            };
        }

        // Retrieves a paginated list of all users
        public async Task<BasePaginatedList<UserResponseModel>> GetAllAsync(int pageIndex, int pageSize)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUser>();

            // Lấy danh sách người dùng không bị xóa
            var query = userRepo.Entities
                .Where(u => !u.UserInfo.IsDelete)
                .OrderBy(u => u.CreatedTime);

            var totalItems = await query.CountAsync();

            // Lấy danh sách người dùng và thông tin liên quan
            var users = await GetUsersWithRolesAsync(query, pageIndex, pageSize);

            return new BasePaginatedList<UserResponseModel>(users, totalItems, pageIndex, pageSize);
        }

        // Lấy danh sách người dùng cùng với vai trò của họ
        private async Task<List<UserResponseModel>> GetUsersWithRolesAsync(IQueryable<ApplicationUser> query, int pageIndex, int pageSize)
        {
            return await query.Skip((pageIndex - 1) * pageSize)
                              .Take(pageSize)
                              .Select(u => new UserResponseModel
                              {
                                  Id = u.Id.ToString(),
                                  UserName = u.UserName,
                                  Email = u.Email,
                                  FullName = u.UserInfo.FullName,
                                  PhoneNumber = u.PhoneNumber,
                                  Role = GetRoleName(u.Id),
                                  CreatedTime = u.CreatedTime,
                                  LastUpdatedTime = u.LastUpdatedTime,
                                  IsDelete = u.UserInfo.IsDelete,
                                  EmailConfirmed = u.EmailConfirmed,
                                  PhoneNumberConfirmed = u.PhoneNumberConfirmed
                              }).ToListAsync();
        }

        // Lấy tên vai trò của người dùng
        private string GetRoleName(Guid userId)
        {
            return _userRoleRepo.Entities
                                .Where(ur => ur.UserId == userId)
                                .Join(_roleRepo.Entities, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                                .FirstOrDefault() ?? "No Role";
        }


        // Registers a new user with the system
        public async Task<string?> RegisterAsync(RegisterModelView model, ClaimsPrincipal userClaims)
        {
            // Kiểm tra người dùng đã tồn tại chưa
            var existingUser = _userRepo.Entities.FirstOrDefault(u => u.UserName == model.Username);
            if (existingUser != null)
            {
                return "User already exists";
            }

            // Hash mật khẩu người dùng
            var hashedPassword = HashHelper.ComputeSha256Hash(model.Password);

            // Lấy thông tin người tạo từ claims
            string? createdBy = userClaims.FindFirst(ClaimTypes.Name)?.Value;

            // Tạo thông tin người dùng mới
            var userInfo = new UserInfo
            {
                FullName = model.FullName,
                BankAccount = model.BankAccount,
                BankAccountName = model.BankAccountName,
                Bank = model.Bank,
                CreatedBy = createdBy,
                LastUpdatedBy = createdBy,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };

            // Lưu thông tin người dùng vào bảng UserInfos
            await _userInfoRepo.InsertAsync(userInfo);
            await _unitOfWork.SaveAsync();

            // Tạo người dùng mới
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Password = model.Password,
                PasswordHash = hashedPassword,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                CreatedBy = createdBy,
                LastUpdatedBy = createdBy,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                Id = Guid.Parse(userInfo.Id)  
            };

            // Lưu người dùng mới vào bảng ApplicationUser
            await _userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            // Kiểm tra và thêm vai trò mặc định nếu chưa tồn tại
            var role = _roleRepo.Entities.FirstOrDefault(r => r.Name == "ConveyorManager");
            if (role == null)
            {
                role = new ApplicationRole
                {
                    Name = "ConveyorManager",
                    NormalizedName = "CONVEYORMANAGER",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow,
                    CreatedBy = createdBy,
                    LastUpdatedBy = createdBy
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
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                CreatedBy = createdBy,
                LastUpdatedBy = createdBy
            };
            await _userRoleRepo.InsertAsync(userRole);
            await _unitOfWork.SaveAsync();

            return null;
        }


        // Logs in the user and returns a JWT token
        public async Task<string?> LoginAsync(LoginModelView model)
        {
            // Find the user in the database
            var user = _userRepo.Entities.FirstOrDefault(u => u.UserName == model.Username);
            if (user == null)
            {
                return "Invalid credentials";
            }

            // Compare the provided password with the stored password hash
            var hashedPassword = HashHelper.ComputeSha256Hash(model.Password);
            if (user.PasswordHash != hashedPassword)
            {
                return "Invalid credentials";
            }

            // Retrieve the user role
            var roleId = _userRoleRepo.Entities
                .Where(r => r.UserId == user.Id)
                .Select(r => r.RoleId)
                .FirstOrDefault();

            if (roleId == default)
            {
                return "User role not found";
            }

            // Retrieve the role name
            var role = _roleRepo.Entities
                .Where(r => r.Id == roleId)
                .Select(r => r.Name)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(role))
            {
                return "User role not found";
            }

            // Generate a JWT token for the user
            string token = TokenHelper.GenerateJwtToken(
                user,
                role,
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]);

            return token;
        }
        // Update a user
        public async Task<string?> UpdateUserAsync(string id, UserUpdateModel updateModel, ClaimsPrincipal userClaims)
        {
            var user = await _userRepo.GetByIdAsync(Guid.Parse(id));
            if (user == null || user.UserInfo.IsDelete)
            {
                return "User not found";
            }

            // Cập nhật các trường cần thiết
            user.Email = updateModel.Email;
            user.PhoneNumber = updateModel.PhoneNumber;
            user.UserInfo.FullName = updateModel.FullName;
            user.LastUpdatedBy = userClaims.Identity?.Name;// sử dụng user claim để lấy tên ra từ token
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return null;
        }
        // Deletes (soft deletes) a user by setting IsDelete to true
        public async Task<string?> DeleteUserAsync(string id, ClaimsPrincipal userClaims)
        {
            var user = await _userRepo.GetByIdAsync(Guid.Parse(id));
            if (user == null || user.UserInfo?.IsDelete == true)
            {
                return "User not found";
            }

            // Đánh dấu là đã xóa
            if (user.UserInfo != null)
            {
                user.UserInfo.IsDelete = true;
                user.UserInfo.DeletedBy = userClaims.Identity?.Name;// sử dụng user claim để lấy tên ra từ token
                user.UserInfo.DeletedTime = CoreHelper.SystemTimeNow;

                await _userInfoRepo.UpdateAsync(user.UserInfo);
                await _unitOfWork.SaveAsync();
            }

            return null;
        }
    }
}