using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AuthModelViews;

namespace XuongMayBE.API.Controllers
{
    /// <summary>
    /// Controller for handling authentication related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor to initialize the AuthController with dependencies.
        /// </summary>
        /// <param name="userService">User service for business logic related to users.</param>
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="model">LoginModelView containing username and password.</param>
        /// <returns>JWT token if authentication is successful, otherwise unauthorized.</returns>
        [HttpPost("auth_account")]
        public async Task<IActionResult> Login(LoginModelView model)
        {
            var token = await _userService.LoginAsync(model);
            
            if (token == "Invalid credentials")
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new { token });
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">LoginModelView containing username and password.</param>
        /// <returns>Returns a status indicating success or failure.</returns>
        [HttpPost("new_account")]
        public async Task<IActionResult> Register([FromBody] RegisterModelView model)
        {
            // Gọi phương thức RegisterAsync từ UserService để đăng ký người dùng mới
            var result = await _userService.RegisterAsync(model, User);

            // Kiểm tra nếu result không phải null, có nghĩa là có lỗi
            if (result != null)
            {
                // Trả về lỗi 400 với thông báo lỗi cụ thể
                return BadRequest(new { message = result });
            }

            // Nếu không có lỗi, tạo và trả về token đăng nhập
            var token = await _userService.LoginAsync(new LoginModelView
            {
                Username = model.Username,
                Password = model.Password
            });

            // Kiểm tra token đã được tạo chưa
            if (token == null)
            {
                return BadRequest(new { message = "Đăng ký thành công nhưng không thể tạo token đăng nhập" });
            }

            // Trả về thành công với token
            return Ok(new { token });
        }



    }
}
