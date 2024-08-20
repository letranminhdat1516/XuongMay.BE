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
        private readonly IGenericRepository<ApplicationUserClaims> _userClaimsRepo;
        private readonly IGenericRepository<ApplicationUserLogins> _userLoginsRepo;
        private readonly IGenericRepository<ApplicationUserTokens> _userTokensRepo;
        private readonly IGenericRepository<ApplicationRoleClaims> _roleClaimsRepo;
        private readonly IGenericRepository<AuditLog> _auditLogRepo;

        // Constructor injects necessary dependencies
        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _userRoleRepo = _unitOfWork.GetRepository<ApplicationUserRoles>();
            _userInfoRepo = _unitOfWork.GetRepository<UserInfo>();
            _userClaimsRepo = _unitOfWork.GetRepository<ApplicationUserClaims>();
            _userLoginsRepo = _unitOfWork.GetRepository<ApplicationUserLogins>();
            _userTokensRepo = _unitOfWork.GetRepository<ApplicationUserTokens>();
            _roleClaimsRepo = _unitOfWork.GetRepository<ApplicationRoleClaims>();
            _auditLogRepo = _unitOfWork.GetRepository<AuditLog>();
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
                DeletedTime = user.DeletedTime,  
                CreatedBy = user.CreatedBy,  
                LastUpdatedBy = user.LastUpdatedBy,  
                DeletedBy = user.DeletedBy,  
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
                .Include(u => u.UserInfo) 
                .Where(u => !u.UserInfo.IsDelete)
                .OrderBy(u => u.CreatedTime);

            var totalItems = await query.CountAsync();

            // Sử dụng phân trang
            var users = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserResponseModel
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.UserInfo != null ? u.UserInfo.FullName : string.Empty,// sử dụng ?.taok
                    PhoneNumber = u.PhoneNumber,
                    CreatedTime = u.CreatedTime,
                    LastUpdatedTime = u.LastUpdatedTime,
                    DeletedTime = u.DeletedTime,
                    CreatedBy = u.CreatedBy,
                    LastUpdatedBy = u.LastUpdatedBy,
                    DeletedBy = u.DeletedBy,
                    IsDelete = u.UserInfo != null && u.UserInfo.IsDelete,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumberConfirmed = u.PhoneNumberConfirmed
                })
                .ToListAsync();

            return new BasePaginatedList<UserResponseModel>(users, totalItems, pageIndex, pageSize);
        }

        // Registers a new user with the system
        // Registers a new user with the system
        public async Task<string?> RegisterAsync(RegisterModelView model, ClaimsPrincipal userClaims)
        {
            // Kiểm tra người dùng đã tồn tại chưa
            var existingUser = _userRepo.Entities.FirstOrDefault(u => u.UserName == model.Username);
            if (existingUser != null)
            {
                return "User đã tồn tại";
            }

            // Hash mật khẩu người dùng
            var hashedPassword = HashHelper.ComputeSha256Hash(model.Password);

            // Lấy thông tin người tạo từ claims
            string? createdBy = userClaims.FindFirst(ClaimTypes.Name)?.Value;

            // Tạo thông tin người dùng mới
            var userInfo = new UserInfo
            {
                FullName = model.FullName,
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
                Id = Guid.NewGuid() // Sử dụng Guid mới
            };

            // Lưu người dùng mới vào bảng ApplicationUser
            await _userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            // Tạo vai trò mặc định nếu chưa tồn tại
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

                // Tạo RoleClaim mặc định cho vai trò này (ManageConveyor)
                var manageConveyorClaim = new ApplicationRoleClaims
                {
                    RoleId = role.Id,
                    ClaimType = "Permission",
                    ClaimValue = "ManageConveyor",
                    CreatedBy = createdBy,
                    LastUpdatedBy = createdBy,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
                };
                await _roleClaimsRepo.InsertAsync(manageConveyorClaim);
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

            // Thêm UserClaim mặc định (ViewConveyor)
            var userClaim = new ApplicationUserClaims
            {
                UserId = user.Id,
                ClaimType = "Permission",
                ClaimValue = "ViewConveyor",
                CreatedBy = createdBy,
                LastUpdatedBy = createdBy,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            await _userClaimsRepo.InsertAsync(userClaim);
            await _unitOfWork.SaveAsync();

            //  lưu Access Token
            string accessToken = TokenHelper.GenerateJwtToken(
                user,
                role.Name,
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]);

            var accessTokenRecord = new ApplicationUserTokens
            {
                UserId = user.Id,
                LoginProvider = "Local",
                Name = "AccessToken",
                Value = accessToken,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            await _userTokensRepo.InsertAsync(accessTokenRecord);
            await _unitOfWork.SaveAsync();

            return null;
        }


        // Logs in the user and returns a JWT token
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

            // Gen Access Token
            string accessToken = TokenHelper.GenerateJwtToken(
                user,
                role,
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]);

            // Check and save Access Token
            var userToken = await _userTokensRepo.Entities
                .FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == "Local");

            if (userToken == null)
            {
                // Tạo mới bản ghi
                userToken = new ApplicationUserTokens
                {
                    UserId = user.Id,
                    LoginProvider = "Local",
                    Name = "AccessToken",
                    Value = accessToken,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
                };
                await _userTokensRepo.InsertAsync(userToken);
            }
            else
            {
                // Cập nhật token nếu đã tồn tại
                userToken.Value = accessToken;
                userToken.LastUpdatedTime = CoreHelper.SystemTimeNow;
                await _userTokensRepo.UpdateAsync(userToken);
            }
            await _unitOfWork.SaveAsync();

            return accessToken;
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
            // Kiểm tra xem GUID có hợp lệ không
            if (!Guid.TryParse(id, out Guid userId))
            {
                return "ID người dùng không hợp lệ.";
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return "Người dùng không tồn tại.";
            }

            if (user.UserInfo != null && user.UserInfo.IsDelete)
            {
                return "Người dùng đã bị xóa.";
            }

            // Đánh dấu là đã xóa
            if (user.UserInfo != null)
            {
                user.UserInfo.IsDelete = true;
                user.UserInfo.DeletedBy = userClaims.Identity?.Name; // sử dụng user claim để lấy tên ra từ token
                user.UserInfo.DeletedTime = CoreHelper.SystemTimeNow;

                await _userInfoRepo.UpdateAsync(user.UserInfo);
                await _unitOfWork.SaveAsync();
            }

            return null;
        }

        // Create role
        public async Task CreateRole(ApplicationRole role, ClaimsPrincipal user)
        {
            role.SetAuditFields(user);
            await _roleRepo.InsertAsync(role);
            await _unitOfWork.SaveAsync();
        }
        // Update role
        public async Task UpdateRole(ApplicationRole role, ClaimsPrincipal user)
        {
            role.SetAuditFields(user);
            await _roleRepo.UpdateAsync(role);
            await _unitOfWork.SaveAsync();
        }
        // DeleteRole
        public async Task DeleteRole(Guid roleId, ClaimsPrincipal user)
        {
            var role = await _roleRepo.GetByIdAsync(roleId);
            if (role != null)
            {
                role.SetDeletedFields(user);
                await _roleRepo.DeleteAsync(role);
                await _unitOfWork.SaveAsync();
            }
        }

    }
}