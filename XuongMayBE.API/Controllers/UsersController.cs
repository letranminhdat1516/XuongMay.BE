using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) {
            _userService = userService;
        }
        [HttpGet()]
        public async Task<IActionResult> Login(int index = 1, int pageSize = 10)
        {
            IList<UserResponseModel> a = await _userService.GetAll();
            return Ok(BaseResponse<IList<UserResponseModel>>.OkResponse(a));
        }
    }
}
