using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.ProductTaskModelViews;

namespace XuongMay.Services.Service
{
    public class OrderTaskService : IOrderTaskService
    {
        #region Attribute
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Orders> _orderRepository;
        private readonly IGenericRepository<OrderTask> _orderTaskRepository;
        private readonly IGenericRepository<Conveyor> _conveyorRepository;
        #endregion

        #region Contructor
        public OrderTaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = unitOfWork.GetRepository<Orders>();
            _orderTaskRepository = unitOfWork.GetRepository<OrderTask>();
            _conveyorRepository = unitOfWork.GetRepository<Conveyor>();
        }
        #endregion

        #region Get All Order Task
        public async Task<IList<OrderTask>> GetAllOrderTask()
        {
            return await _orderTaskRepository.GetAllAsync();
        }

        #endregion

        #region Get All Order Task With Paging
        public Task<BasePaginatedList<OrderTask>> GetAllOrderTaskWithPaging(int index, int pageSize)
        {
            var orderTask = _orderTaskRepository.Entities.Where(ord => ord.DeletedTime == null);
            var orderTaskPaging = _orderTaskRepository.GetPagging(orderTask, index, pageSize);
            return orderTaskPaging;
        }

        #endregion

        #region Get Order Task By Id
        public async Task<OrderTask?> GetOrderTaskById(object id)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(id);
            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }
            return orderTask;
        }

        #endregion

        #region Insert Order Task
        public async Task InsertOrderTask(OrderTask obj)
        {
            if (obj == null
                || obj.OrderId == null
                || obj.ConveyorId == null)
            {
                throw new BaseException.BadRequestException("Bad Request", "Thêm dữ liệu thất bại");
            }

            var conveyor = _conveyorRepository.GetById(obj.ConveyorId);
            bool checkOrder = IsOrderTaskQuantityOutOfOrderRange(obj.OrderId, obj.Quantity);
            bool checkConveyor = IsOrderTaskQuantityOutOfConveyorRange(obj.ConveyorId, obj.Quantity);

            if (conveyor == null)
            {
                throw new BaseException.BadRequestException("Bad Request", "Không tìm thấy băng chuyền");
            }

            if (conveyor.IsWorking)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Băng chuyền này đang hoạt động");
            }

            if (checkOrder)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Số lượng yêu câu vượt quá số lượng đơn hàng");
            }

            if (checkConveyor)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Số lượng đơn hàng vượt quá số lượng tối đa của băng chuyền");
            }

            conveyor.IsWorking = true;
            await _orderTaskRepository.InsertAsync(obj);
            await _conveyorRepository.UpdateAsync(conveyor);
            await SaveAsync();
        }
        #endregion

        #region Update Order Task
        public async Task UpdateOrderTask(string id, OrderTaskRequestModel obj)
        {
            var orderTask = await GetOrderTaskById(id);
            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }
            orderTask.TaskNote = obj.TaskNote;
            orderTask.Quantity = obj.Quantity;
            orderTask.Status = obj.Status;
            orderTask.LastUpdatedTime = CoreHelper.SystemTimeNow;
            orderTask.LastUpdatedBy = obj.UpdateBy;
            await _orderTaskRepository.UpdateAsync(orderTask);
            await SaveAsync();
        }
        #endregion

        #region Delete Order Task
        public async Task DeleteOrderTask(object id, string deleteBy)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(id);
            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }
            orderTask.DeletedBy = deleteBy;
            orderTask.DeletedTime = CoreHelper.SystemTimeNow;
            await _orderTaskRepository.UpdateAsync(orderTask);
            //await _orderTaskRepository.DeleteAsync(id);
            await SaveAsync();
        }
        #endregion

        #region Update Order Task Status
        public async Task UpdateOrderTaskStatus(string id, OrderTaskRequestModel obj)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(id);
            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }
            orderTask.Status = obj.Status;
            orderTask.LastUpdatedTime = CoreHelper.SystemTimeNow;
            orderTask.LastUpdatedBy = obj.UpdateBy ?? string.Empty;
            await _orderTaskRepository.UpdateAsync(orderTask);
            await SaveAsync();
        }
        #endregion

        #region Compare the quantity of order task and order
        public bool IsOrderTaskQuantityOutOfOrderRange(string orderId, int taskQuantity)
        {
            bool result = false;
            var order = _orderRepository.GetById(orderId);

            if (order == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy đơn hàng");
            }

            result = order.Quantity >= taskQuantity;

            return result;
        }
        #endregion

        #region Compare conveyor max quantity and order task
        public bool IsOrderTaskQuantityOutOfConveyorRange(string conveyorId, int taskQuantity)
        {
            bool result = false;
            var conveyor = _conveyorRepository.GetById(conveyorId);

            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            result = conveyor.MaxQuantity >= taskQuantity;

            return result;
        }
        #endregion

        #region Save Async
        public async Task SaveAsync()
        {
            await _unitOfWork.SaveAsync();
        }

        #endregion
    }
}
