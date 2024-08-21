using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.RoleModelView;
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
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
            // Lấy vai trò của người dùng
            var roleId = _userRoleRepo.Entities
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .FirstOrDefault();

            string? roleName = null;
            if (roleId != default)
            {
                roleName = await _roleRepo.Entities
                    .Where(r => r.Id == roleId)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();
            }

            return new UserResponseModel
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.UserInfo?.FullName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                CreatedTime = user.CreatedTime,
                LastUpdatedTime = user.LastUpdatedTime,
                DeletedTime = user.DeletedTime,
                CreatedBy = user.CreatedBy,
                LastUpdatedBy = user.LastUpdatedBy,
                DeletedBy = user.DeletedBy,
                IsDelete = user.UserInfo?.IsDelete ?? false,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Role = roleName
            };
        }


        // Retrieves a paginated list of all users
        public async Task<BasePaginatedList<UserResponseModel>> GetAllAsync(int pageIndex, int pageSize)
        {
            // Truy vấn danh sách người dùng, bao gồm cả những người dùng không có UserInfo
            var query = _userRepo.Entities
                .Include(u => u.UserInfo)
                .Where(u => u.UserInfo == null || !u.UserInfo.IsDelete)
                .OrderBy(u => u.CreatedTime);

            // Đếm tổng số phần tử
            var totalItems = await query.CountAsync();

            // Kiểm tra nếu pageIndex và pageSize hợp lệ
            if (pageIndex < 1 || pageSize < 1)
            {
                throw new ArgumentException("PageIndex và PageSize phải lớn hơn 0.");
            }

            // Lấy danh sách người dùng với phân trang
            var users = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userResponseModels = new List<UserResponseModel>();

            foreach (var user in users)
            {
                // Lấy role ID từ bảng ApplicationUserRoles
                var roleId = await _userRoleRepo.Entities
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .FirstOrDefaultAsync();

                // Lấy tên role từ bảng ApplicationRole
                string? roleName = null;
                if (roleId != default)
                {
                    roleName = await _roleRepo.Entities
                        .Where(r => r.Id == roleId)
                        .Select(r => r.Name)
                        .FirstOrDefaultAsync();
                }

                var userResponseModel = new UserResponseModel
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.UserInfo != null ? user.UserInfo.FullName : string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    CreatedTime = user.CreatedTime,
                    LastUpdatedTime = user.LastUpdatedTime,
                    DeletedTime = user.DeletedTime,
                    CreatedBy = user.CreatedBy,
                    LastUpdatedBy = user.LastUpdatedBy,
                    DeletedBy = user.DeletedBy,
                    IsDelete = user.UserInfo != null && user.UserInfo.IsDelete,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    Role = roleName 
                };

                userResponseModels.Add(userResponseModel);
            }

            return new BasePaginatedList<UserResponseModel>(userResponseModels, totalItems, pageIndex, pageSize);
        }

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
                UserInfoId = userInfo.Id,
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

            // Tạo và lưu Access Token
            string accessToken = TokenHelper.GenerateJwtToken(
                user,
                role.Name,
                new List<string> { "ViewConveyor" },
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"] 
            );


            var accessTokenRecord = new ApplicationUserTokens
            {
                UserId = user.Id,
                LoginProvider = "Local",
                Name = "AccessToken",
                Value = accessToken,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            accessTokenRecord.SetAuditFields(userClaims);
            await _userTokensRepo.InsertAsync(accessTokenRecord);
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

            // Get permissions from UserClaims and RoleClaims
            var userClaims = await _userClaimsRepo.Entities
                .Where(uc => uc.UserId == user.Id && uc.ClaimType == "Permission")
                .Select(uc => uc.ClaimValue)
                .ToListAsync();

            var roleClaims = await _roleClaimsRepo.Entities
                .Where(rc => rc.RoleId == roleId && rc.ClaimType == "Permission")
                .Select(rc => rc.ClaimValue)
                .ToListAsync();

            // Combine both UserClaims and RoleClaims
            var permissions = userClaims.Union(roleClaims).ToList();

            // Gen Access Token
            string accessToken = TokenHelper.GenerateJwtToken(
                user,
                role,
                permissions,
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

            user.Email = updateModel.Email;
            user.PhoneNumber = updateModel.PhoneNumber;
            user.UserInfo.FullName = updateModel.FullName;
            user.UserInfo.BankAccount = updateModel.BankAccount;
            user.UserInfo.BankAccountName = updateModel.BankAccountName;
            user.UserInfo.Bank = updateModel.Bank;
            user.LastUpdatedBy = userClaims.Identity?.Name; // sử dụng user claim để lấy tên ra từ token
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return null;
        }

        // Deletes a user by setting IsDelete to true
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
        public async Task CreateRole(RoleCreateModelView model, ClaimsPrincipal user)
        {
            var role = new ApplicationRole
            {
                Name = model.Name,
                NormalizedName = model.NormalizedName,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            role.SetAuditFields(user);
            await _roleRepo.InsertAsync(role);
            await _unitOfWork.SaveAsync();
        }

        // Update role
        public async Task UpdateRole(Guid roleId, RoleUpdateModelView model, ClaimsPrincipal user)
        {
            var role = await _roleRepo.GetByIdAsync(roleId);

            if (role == null)
            {
                throw new Exception($"Role ID '{roleId}' không tồn tại.");
            }

            role.Name = model.Name;
            role.NormalizedName = model.NormalizedName;

            role.SetAuditFields(user);
            await _roleRepo.UpdateAsync(role);
            await _unitOfWork.SaveAsync();
        }
        // Get role
        public async Task<List<RoleResponseModel>> GetRolesAsync()
        {
            var roles = await _roleRepo.Entities
                .Where(r => r.DeletedTime == null) // delete time null là chưa xoá 
                .Select(r => new RoleResponseModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    NormalizedName = r.NormalizedName,
                    CreatedBy = r.CreatedBy,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedBy = r.LastUpdatedBy,
                    LastUpdatedTime = r.LastUpdatedTime
                })
                .ToListAsync();

            return roles;
        }
        // Delete role
        public async Task DeleteRole(Guid roleId, ClaimsPrincipal user)
        {
            var role = await _roleRepo.GetByIdAsync(roleId);
            if (role != null)
            {
                role.SetDeletedFields(user);
               

                await _roleRepo.UpdateAsync(role); 
                await _unitOfWork.SaveAsync();
            }
        }
        // Set role user
        public async Task SetRoleAsync(Guid userId, string roleName, ClaimsPrincipal adminUser)
        {
            // Tìm vai trò theo tên
            var role = await _roleRepo.Entities
                .FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper());

            if (role == null)
            {
                throw new KeyNotFoundException($"Vai trò '{roleName}' không tồn tại.");
            }

            // Tìm người dùng theo ID
            var user = await _userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"Người dùng với ID '{userId}' không tồn tại.");
            }

            // Xóa các vai trò cũ của người dùng
            var existingUserRoles = await _userRoleRepo.Entities
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _userRoleRepo.DeleteAsync(existingUserRoles);

            // Gán vai trò mới cho người dùng
            var userRole = new ApplicationUserRoles
            {
                UserId = userId,
                RoleId = role.Id
            };

            userRole.SetAuditFields(adminUser);
            await _userRoleRepo.InsertAsync(userRole);

            // Lưu thay đổi
            await _unitOfWork.SaveAsync();
        }

        // Delete role user by setting role is "not avaiable"
        public async Task DeleteUserRole(Guid userId, ClaimsPrincipal adminUser)
        {
            // Tìm người dùng theo ID
            var user = await _userRepo.Entities
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"Người dùng với ID '{userId}' không tồn tại.");
            }

            // Xóa các vai trò cũ của người dùng
            var existingUserRoles = await _userRoleRepo.Entities
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            if (existingUserRoles.Any())
            {
                foreach (var existingUserRole in existingUserRoles)
                {
                  
                    existingUserRole.SetDeletedFields(adminUser);

                    // Lưu thay đổi cập nhật
                    await _userRoleRepo.UpdateAsync(existingUserRole);
                }
            }

            var notAvailableRole = await EnsureNotAvailableRoleExistsAsync();

            // Gán vai trò "Not Available" cho người dùng
            var userRole = new ApplicationUserRoles
            {
                UserId = userId,
                RoleId = notAvailableRole.Id
            };

            userRole.SetAuditFields(adminUser);
            await _userRoleRepo.InsertAsync(userRole);

            // Lưu thay đổi
            await _unitOfWork.SaveAsync();
        }


        // Create role "not avaiable"
        private async Task<ApplicationRole> EnsureNotAvailableRoleExistsAsync()
        {
            var notAvailableRole = await _roleRepo.Entities
                .FirstOrDefaultAsync(r => r.NormalizedName == "NOT AVAILABLE");

            if (notAvailableRole == null)
            {
                notAvailableRole = new ApplicationRole
                {
                    Name = "Not Available",
                    NormalizedName = "NOT AVAILABLE",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
                };

                await _roleRepo.InsertAsync(notAvailableRole);
                await _unitOfWork.SaveAsync();
            }

            return notAvailableRole;
        }
    }
}