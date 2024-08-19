using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Constants;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, ConveyorManager")]
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
        /// Chỉ dành cho Admin.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id, User);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(result));
            }
            return Ok(BaseResponse<string>.OkResponse("Xóa người dùng thành công"));
        }
    }
}
