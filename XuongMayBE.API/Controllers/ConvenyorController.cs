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

        [HttpGet()]
        [SwaggerOperation(Summary = "Lấy danh sách các băng chuyền có phân trang")]
        public async Task<IActionResult> GetConveyorPaging(int index = 1, int pageSize = 5)
        {
            BasePaginatedList<Conveyor> conveyors = await _conveyorService.GetAllConveyorPaging(index, pageSize);
            return Ok(BaseResponse<BasePaginatedList<Conveyor>>.OkResponse(conveyors));
        }

        [HttpPost()]
        [SwaggerOperation(Summary = "Tạo mới băng chuyền")]
        public async Task<IActionResult> InsertConveyor([FromBody] ConveyorRequestModel request)
        {
            try
            {
                Conveyor conveyor = new Conveyor();
                conveyor.ConveyorName = request.ConveyorName;
                conveyor.ConveyorCode = request.ConveyorCode;
                conveyor.ConveyorNumber = request.ConveyorNumber;
                await _conveyorService.InsertNewConveyor(conveyor);
                var response = BaseResponse<string>.OkResponse("Tạo mới băng chuyền thành công");
                return Ok(response);
            }
            catch (BaseException.ErrorException ex)
            {
                var response = BaseResponse<string>.ErrorResponse(ex.ErrorDetail.ErrorMessage?.ToString());
                return BadRequest(response);
            }

        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin băng chuyền")]
        public async Task<IActionResult> UpdateConveyor(string id, [FromBody] ConveyorRequestModel request)
        {
            try
            {
                await _conveyorService.UpdateConveyor(id, request);
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
    }
}
