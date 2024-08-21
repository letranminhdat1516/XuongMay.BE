using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Constants;
using XuongMay.ModelViews.RoleModelView;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách người dùng có phân trang.
        /// Chỉ dành cho Admin và ConveyorManager.
        /// </summary>
        [HttpGet("Users")]
        [Authorize(Policy = "ViewPolicy")]
        public async Task<IActionResult> GetAllUsers(int pageIndex = 1, int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(pageIndex, pageSize);
            return Ok(BaseResponse<BasePaginatedList<UserResponseModel>>.OkResponse(users));
        }

        /// <summary>
        /// Xem chi tiết thông tin người dùng.
        /// dành cho Admin và ConveyorManager.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "ViewPolicy")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(BaseResponse<string>.ErrorResponse("Người dùng không tồn tại"));
            }
            return Ok(BaseResponse<UserResponseModel>.OkResponse(user));
        }

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateModel updateModel)
        {
            var result = await _userService.UpdateUserAsync(id, updateModel, User);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(result));
            }
            return Ok(BaseResponse<string>.OkResponse("Cập nhật người dùng thành công"));
        }

        /// <summary>
        /// Xóa người dùng (chuyển trạng thái IsDelete).
        /// Chỉ dành cho Admin.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id, User);
            if (!string.IsNullOrEmpty(result))
            {
                // Trả về lỗi chi tiết
                return NotFound(new { message = result });
            }
            return Ok(new { message = "Xóa người dùng thành công" });
        }
        /// <summary>
        /// Xem tất cả các role.
        /// Chỉ dành cho Admin.
        /// </summary>
        [HttpGet("roles")]
        [Authorize(Policy = "ViewPolicy")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userService.GetRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Tạo Role.
        /// Chỉ dành cho Admin.
        /// </summary>
        [HttpPost("roles")]
        [Authorize(Policy = "InsertPolicy")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateModelView model)
        {
            if (model == null)
            {
                return BadRequest("Role không được rỗng.");
            }

            await _userService.CreateRole(model, User);

            return Ok("Tạo vai trò thành công.");
        }

        /// <summary>
        /// Sửa Role.
        /// Chỉ dành cho Admin.
        /// </summary>
        [HttpPut("roles/{roleId}")]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] RoleUpdateModelView model)
        {
            if (model == null)
            {
                return BadRequest("Thông tin vai trò không hợp lệ.");
            }

            // Giả sử model.Id đã đúng và khớp với roleId trong URL
            await _userService.UpdateRole(roleId, model, User);

            return Ok("Cập nhật vai trò thành công.");
        }


        /// <summary>
        /// Xoá Role (chuyển trạng thái sang đã xóa).
        /// Chỉ dành cho Admin.
        /// </summary>
        [HttpDelete("roles/{roleId}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            await _userService.DeleteRole(roleId, User);
            return Ok("Xóa vai trò thành công.");
        }
        /// <summary>
        /// Đặt vai trò cho người dùng.
        /// Chỉ dành cho Admin.
        /// </summary>
        /// <param name="model">Thông tin về vai trò và người dùng.</param>
        /// <returns>Trả về kết quả của việc đặt vai trò.</returns>
        [HttpPut("roles/set")]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> SetRole([FromBody] RoleSetModelView model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.RoleName) || model.UserId == Guid.Empty)
            {
                return BadRequest("Thông tin vai trò hoặc người dùng không hợp lệ.");
            }

            try
            {
                await _userService.SetRoleAsync(model.UserId, model.RoleName, User);

                return Ok("Đặt vai trò thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi khi đặt vai trò: {ex.Message}");
            }
        }
        /// <summary>
        /// Cập nhật vai trò cho người dùng thành not avaiable.
        /// Chỉ dành cho Admin.
        /// </summary>
        /// <returns>Trả về kết quả của việc xoá vai trò.</returns>
        [HttpDelete("roles/delete-role-user")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteUserRole(Guid userId)
        {
            try
            {
                await _userService.DeleteUserRole(userId, User);
                return Ok("Xóa vai trò thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi khi xóa vai trò: {ex.Message}");
            }
        }

        [HttpPost("assignRoleViewToUser")]
        [Authorize(Policy = "InsertPolicy")]
        public async Task<IActionResult> AssignRoleViewToUser(string id)
        {
            try
            {
                await _userService.SetUserCanView(id, User.Identity?.Name ?? "");
                return Ok(BaseResponse<string>.OkResponse("Phân quyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        [HttpPost("assignRoleEditToUser")]
        [Authorize(Policy = "InsertPolicy")]
        public async Task<IActionResult> AssignRoleEditToUser(string id)
        {
            try
            {
                await _userService.SetUserCanEdit(id, User.Identity?.Name ?? "");
                return Ok(BaseResponse<string>.OkResponse("Phân quyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        [HttpPost("assignRoleDeleteToUser")]
        [Authorize(Policy = "InsertPolicy")]
        public async Task<IActionResult> AssignRoleDeleteToUser(string id)
        {
            try
            {
                await _userService.SetUserCanDelete(id, User.Identity?.Name ?? "");
                return Ok(BaseResponse<string>.OkResponse("Phân quyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }

        [HttpPost("assignRoleInsertToUser")]
        [Authorize(Policy = "InsertPolicy")]
        public async Task<IActionResult> AssignRoleInsertToUser(string id)
        {
            try
            {
                await _userService.SetUserCanInsert(id, User.Identity?.Name ?? "");
                return Ok(BaseResponse<string>.OkResponse("Phân quyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
    }
}
