using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.ModelViews.ConveyorModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConvenyorController : ControllerBase
    {
        private readonly IConveyorService _conveyorService;

        public ConvenyorController(IConveyorService conveyorService)
        {
            _conveyorService = conveyorService;
        }

        #region Lấy danh sách các băng chuyền
        [HttpGet()]
        [SwaggerOperation(Summary = "Lấy danh sách các băng chuyền có phân trang")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> GetConveyorPaging(int index = 1, int pageSize = 10)
        {
            BasePaginatedList<Conveyor> conveyors = await _conveyorService.GetAllConveyorPaging(index, pageSize);
            return Ok(BaseResponse<BasePaginatedList<Conveyor>>.OkResponse(conveyors));
        }
        #endregion

        #region Lấy thông tin băng chuyền theo filter
        [HttpGet("filter")]
        [SwaggerOperation(Summary = "Lấy thông tin của băng chuyền theo filter")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> GetOneConveyor(string keyword = "", int index = 1, int pageSize = 10)
        {
            try
            {
                var conveyors = await _conveyorService.GetConveyorByFilter(keyword, index, pageSize);
                return Ok(BaseResponse<BasePaginatedList<Conveyor>>.OkResponse(conveyors));
            }
            catch (BaseException.ErrorException ex)
            {
                return NotFound(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Thêm mới băng chuyền
        [HttpPost()]
        [SwaggerOperation(Summary = "Tạo mới băng chuyền")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> InsertConveyor([FromBody] ConveyorRequestModel request)
        {
            try
            {
                request.CreateBy = User.Identity?.Name ?? "";
                await _conveyorService.InsertNewConveyor(request);
                return Ok(BaseResponse<string>.OkResponse("Tạo mới băng chuyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }

        }
        #endregion

        #region Cập nhật thông tin băng chuyền
        [HttpPut()]
        [SwaggerOperation(Summary = "Cập nhật thông tin băng chuyền")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ConveyorManager")]
        public async Task<IActionResult> UpdateConveyor([FromBody] ConveyorUpdateModel request)
        {
            try
            {
                request.UpdateBy = User.Identity?.Name ?? "";
                await _conveyorService.UpdateConveyor(request);
                return Ok(BaseResponse<string>.OkResponse("Cập nhật băng chuyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return NotFound(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Xóa thông tin băng chuyền
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa thông tin băng chuyền")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,ConveyorManager")]
        public async Task<IActionResult> DeleteConveyor(string id)
        {
            try
            {
                await _conveyorService.DeleteConveyor(id, User.Identity?.Name ?? "");
                return Ok(BaseResponse<string>.OkResponse("Xóa băng chuyền thành công"));
            }
            catch (BaseException.ErrorException ex)
            {
                return NotFound(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion
    }
}
