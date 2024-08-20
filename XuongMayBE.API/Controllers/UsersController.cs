using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Constants;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, ConveyorManager")]
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
        public async Task<IActionResult> GetAllUsers(int pageIndex = 1, int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(pageIndex, pageSize);
            return Ok(BaseResponse<BasePaginatedList<UserResponseModel>>.OkResponse(users));
        }

        /// <summary>
        /// Xem chi tiết thông tin người dùng.
        /// Chỉ dành cho Admin và ConveyorManager.
        /// </summary>
        [HttpGet("{id}")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
        [HttpPost("roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] ApplicationRole role)
        {
            if (role == null)
            {
                return BadRequest("Role không được rỗng.");
            }

            await _userService.CreateRole(role, User);

            return Ok("Tạo vai trò thành công.");
        }

        [HttpPut("roles/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] ApplicationRole role)
        {
            if (role == null || role.Id != roleId)
            {
                return BadRequest("Thông tin vai trò không hợp lệ.");
            }

            await _userService.UpdateRole(role, User);

            return Ok("Cập nhật vai trò thành công.");
        }

        [HttpDelete("roles/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            await _userService.DeleteRole(roleId, User);

            return Ok("Xóa vai trò thành công.");
        }

    }
}
