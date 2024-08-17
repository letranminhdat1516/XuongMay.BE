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
        public async Task<IActionResult> InsertConveyor([FromBody] ConveyorCreateModel request)
        {
            Conveyor conveyor = new Conveyor();
            conveyor.ConveyorName = request.ConveyorName;
            conveyor.ConveyorCode = request.ConveyorCode;
            conveyor.ConveyorNumber = request.ConveyorNumber;
            await _conveyorService.InsertNewConveyor(conveyor);
            return Ok(BaseResponse<String>.OkResponse("Tạo mới băng chuyền thành công"));
        }

        [HttpPut()]
        [SwaggerOperation(Summary = "Cập nhật thông tin băng chuyền")]
        public async Task<IActionResult> UpdateConveyor([FromBody] Conveyor request)
        {
            await _conveyorService.UpdateConveyor(request);
            return Ok(BaseResponse<String>.OkResponse("Cập nhật băng chuyền thành công"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa thông tin băng chuyền băng chuyền")]
        public async Task<IActionResult> DeleteConveyor(string id)
        {
            await _conveyorService.DeleteConveyor(id);
            return Ok(BaseResponse<String>.OkResponse("Xóa băng chuyền thành công"));
        }
    }
}
