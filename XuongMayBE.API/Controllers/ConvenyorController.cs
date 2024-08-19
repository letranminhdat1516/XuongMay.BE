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
        public async Task<IActionResult> GetConveyorPaging(int index = 1, int pageSize = 10)
        {
            BasePaginatedList<Conveyor> conveyors = await _conveyorService.GetAllConveyorPaging(index, pageSize);
            var response = BaseResponse<BasePaginatedList<Conveyor>>.OkResponse(conveyors);
            return Ok(response);
        }
        #endregion

        #region Lấy thông tin băng chuyền theo filter
        [HttpGet("filter")]
        [SwaggerOperation(Summary = "Lấy thông tin của băng chuyền theo filter")]
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
        public async Task<IActionResult> InsertConveyor([FromBody] ConveyorRequestModel request)
        {
            try
            {
                request.CreateBy = "KietPHG";
                await _conveyorService.InsertNewConveyor(request);
                var response = BaseResponse<string>.OkResponse("Tạo mới băng chuyền thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }

        }
        #endregion

        #region Cập nhật thông tin băng chuyền
        [HttpPut()]
        [SwaggerOperation(Summary = "Cập nhật thông tin băng chuyền")]
        public async Task<IActionResult> UpdateConveyor([FromBody] ConveyorUpdateModel request)
        {
            try
            {
                request.UpdateBy = "KietPHG";
                await _conveyorService.UpdateConveyor(request);
                return Ok(BaseResponse<string>.OkResponse("Cập nhật băng chuyền thành công"));
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion

        #region Xóa thông tin băng chuyền
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa thông tin băng chuyền")]
        public async Task<IActionResult> DeleteConveyor(string id)
        {
            try
            {
                await _conveyorService.DeleteConveyor(id, "KietPHG");
                return Ok(BaseResponse<string>.OkResponse("Xóa băng chuyền thành công"));
            }
            catch (BaseException.ErrorException ex) when (ex.StatusCode == 404)
            {
                return NotFound(BaseResponse<string>.NotFoundResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString()));
            }
        }
        #endregion
    }
}
