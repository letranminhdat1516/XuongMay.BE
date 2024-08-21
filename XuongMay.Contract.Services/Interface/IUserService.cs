using System.Security.Claims;
using XuongMay.Core;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.UserModelViews;
using XuongMay.ModelViews.RoleModelView;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống theo Page size.
        /// </summary>
        Task<BasePaginatedList<UserResponseModel>> GetAllAsync(int pageIndex, int pageSize);

        /// <summary>
        /// Lấy chi tiết người dùng theo ID.
        /// </summary>
        Task<UserResponseModel?> GetByIdAsync(string id);

        /// <summary>
        /// Đăng ký một người dùng mới.
        /// </summary>
        Task<string?> RegisterAsync(RegisterModelView model, ClaimsPrincipal userClaims);

        /// <summary>
        /// Đăng nhập người dùng và trả về JWT token.
        /// </summary>
        Task<string?> LoginAsync(LoginModelView model);

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        Task<string?> UpdateUserAsync(string id, UserUpdateModel updateModel, ClaimsPrincipal userClaims);

        /// <summary>
        /// Xóa người dùng (chuyển trạng thái IsDelete).
        /// </summary>
        Task<string?> DeleteUserAsync(string id, ClaimsPrincipal userClaims);

        /// <summary>
        /// Tạo một vai trò mới.
        /// </summary>
        Task CreateRole(RoleCreateModelView model, ClaimsPrincipal user);

        /// <summary>
        /// Cập nhật một vai trò hiện có.
        /// </summary>
        Task UpdateRole(Guid roleId, RoleUpdateModelView model, ClaimsPrincipal user);

        /// <summary>
        /// Xóa một vai trò theo ID.
        /// </summary>
        Task DeleteRole(Guid roleId, ClaimsPrincipal user);
        /// <summary>
        /// Cập nhật người dùng theo vai trò.
        /// </summary>
        Task SetRoleAsync(Guid userId, string roleName, ClaimsPrincipal adminUser);
    }
}
