using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.OrderTaskModelViews;
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
        public Task<BasePaginatedList<OrderTask>> GetAllOrderTask(int index, int pageSize)
        {
            if (index < 0 || pageSize < 0)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Index, PageSize phải lớn hơn 1");
            }
            if (index > pageSize)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Index phải nhỏ hơn PageSize");
            }
            var orderTask = _orderTaskRepository.Entities.Where(ord => !ord.IsDelete);
            return _orderTaskRepository.GetPagging(orderTask, index, pageSize);
        }

        #endregion

        #region Get All Order Task By Filter
        public async Task<BasePaginatedList<OrderTask>> GetAllOrderTaskByFiler(string keyword, int index, int pageSize)
        {
            if (index < 0 || pageSize < 0)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Index, PageSize phải lớn hơn 1");
            }
            if (index > pageSize)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Index phải nhỏ hơn PageSize");
            }
            var orderTasks = _orderTaskRepository
                            .Entities
                            .Where(ord => !ord.IsDelete
                            && (ord.Status.Contains(keyword) || string.IsNullOrWhiteSpace(keyword)));
            return await _orderTaskRepository.GetPagging(orderTasks, index, pageSize);
        }
        #endregion

        #region Insert Order Task
        public async Task InsertOrderTask(OrderTaskRequestModel obj)
        {
            if (obj?.OrderId == null || obj?.ConveyorId == null)
            {
                throw new BaseException.BadRequestException("Bad Request", "Vui lòng chọn băng chuyền và đơn hàng");
            }

            var conveyor = await _conveyorRepository.GetByIdAsync(obj.ConveyorId);

            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            var orderTask = new OrderTask
            {
                OrderId = obj.OrderId,
                ConveyorId = obj.ConveyorId,
                TaskNote = obj.TaskNote,
                Quantity = obj.Quantity,
                CreatedBy = obj.CreateBy,
            };

            await ValidateRequestData(conveyor, orderTask);

            conveyor.IsWorking = true;

            await _orderTaskRepository.InsertAsync(orderTask);
            await _conveyorRepository.UpdateAsync(conveyor);
            await SaveAsync();
        }
        #endregion

        #region Update Order Task
        public async Task UpdateOrderTask(OrderTaskUpdateModel obj)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(obj.OrderTaskId);
            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }

            var conveyor = await _conveyorRepository.GetByIdAsync(orderTask.ConveyorId);

            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            orderTask.Quantity = obj.Quantity;
            orderTask.TaskNote = obj.TaskNote;
            orderTask.LastUpdatedBy = obj.UpdateBy;
            orderTask.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await ValidateRequestData(conveyor, orderTask);

            await _orderTaskRepository.UpdateAsync(orderTask);

            if (!conveyor.IsWorking)
            {
                conveyor.IsWorking = true;
                await _conveyorRepository.UpdateAsync(conveyor);
            }

            await SaveAsync();
        }
        #endregion

        #region Update Order Task Status
        public async Task UpdateOrderTaskStatus(OrderTaskUpdateModel obj)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(obj.OrderTaskId);

            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }

            var conveyor = await _conveyorRepository.GetByIdAsync(orderTask.ConveyorId);

            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            orderTask.Status = obj.Status;
            orderTask.LastUpdatedTime = CoreHelper.SystemTimeNow;
            orderTask.LastUpdatedBy = obj.UpdateBy ?? string.Empty;
            conveyor.IsWorking = obj.Status == "Processing" ? true : false;
            await _orderTaskRepository.UpdateAsync(orderTask);
            await _conveyorRepository.UpdateAsync(conveyor);
            await SaveAsync();
        }
        #endregion

        #region Update Order Task Quantity Complete
        public async Task UpdateOrderTaskCompleteQuantity(OrderTaskUpdateCompleteQuantity obj)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(obj.OrderTaskId);
            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }

            var conveyor = await _conveyorRepository.GetByIdAsync(orderTask.ConveyorId);
            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            if (obj.Quantity > orderTask.Quantity)
            {
                throw new BaseException.ErrorException(400, "Bad Request", $"Số lượng hoàn thành không lớn hơn số lượng đang thực hiện (Số lượng đang thực hiện: {orderTask.Quantity})");
            }

            if (obj.Quantity < 1)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Số lượng hoàn thành phải lớn hơn 0");
            }

            orderTask.CompleteQuantity = obj.Quantity;
            orderTask.Quantity -= obj.Quantity;
            orderTask.LastUpdatedBy = obj.UpdateBy;
            orderTask.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _orderTaskRepository.UpdateAsync(orderTask);
            if (orderTask.Quantity == 0)
            {
                conveyor.IsWorking = false;
                conveyor.LastUpdatedTime = CoreHelper.SystemTimeNow;
                conveyor.LastUpdatedBy = obj.UpdateBy;
                await _conveyorRepository.UpdateAsync(conveyor);
            }
            await SaveAsync();
        }
        #endregion

        #region Delete Order Task
        public async Task DeleteOrderTask(object orderTaskId, string deleteBy)
        {
            var orderTask = await _orderTaskRepository.GetByIdAsync(orderTaskId);

            if (orderTask == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy nhiệm vụ");
            }

            var conveyor = await _conveyorRepository.GetByIdAsync(orderTask.ConveyorId);
            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            if (orderTask.IsDelete)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Nhiệm vụ này đã bị xóa không thể xóa");
            }

            if (conveyor.IsWorking)
            {
                throw new BaseException.ErrorException(400, "Bad Request", "Băng chuyền đang hoạt động không thể xóa");
            }


            orderTask.DeletedBy = deleteBy;
            orderTask.DeletedTime = CoreHelper.SystemTimeNow;
            orderTask.IsDelete = true;
            await _orderTaskRepository.UpdateAsync(orderTask);
            await SaveAsync();
        }
        #endregion

        #region Save Async
        public async Task SaveAsync()
        {
            await _unitOfWork.SaveAsync();
        }

        #endregion

        // So sánh các dữ liệu

        #region Compare the quantity of order task and order
        public async Task<bool> IsOrderTaskQuantityOutOfOrderRange(string orderId, int taskQuantity)
        {
            bool result = false;
            int orderQuantity = await GetOrderQuantity(orderId);

            result = orderQuantity >= taskQuantity;

            return result;
        }
        #endregion

        #region Compare conveyor max quantity and order task
        public async Task<bool> IsOrderTaskQuantityOutOfConveyorRange(string conveyorId, int taskQuantity)
        {
            bool result = false;
            int maxConveyor = await GetMaxConveyor(conveyorId);

            result = taskQuantity > maxConveyor;

            return result;
        }
        #endregion

        #region Total Quantity OrderTask Of Order
        public Task<int> TotalQuantityOrderTaskOfOrder(object orderId)
        {
            int totalQuantity = _orderTaskRepository.Entities.Where(ordTask => ordTask.OrderId.Equals(orderId) && ordTask.DeletedTime == null).Sum(ord => ord.Quantity);
            return Task.FromResult(totalQuantity);
        }
        #endregion

        #region Get Order Quantity
        public async Task<int> GetOrderQuantity(object id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy đơn hàng");
            }

            return order.Quantity;
        }
        #endregion

        #region Get Max Conveyor
        public async Task<int> GetMaxConveyor(object conveyorId)
        {
            var conveyor = await _conveyorRepository.GetByIdAsync(conveyorId);

            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }
            return conveyor.MaxQuantity;
        }
        #endregion

        // Kiểm tra dữ liệu đầu vào

        #region Validate request order task data
        public async Task ValidateRequestData(Conveyor conveyor, OrderTask obj)
        {
            bool checkOrder = await IsOrderTaskQuantityOutOfOrderRange(obj.OrderId, obj.Quantity);
            bool checkConveyor = await IsOrderTaskQuantityOutOfConveyorRange(obj.ConveyorId, obj.Quantity);
            bool checkQuantity = obj.Quantity > 1;
            int totalQuantityOfOrderTask = await TotalQuantityOrderTaskOfOrder(obj.OrderId);
            int orderQuantity = await GetOrderQuantity(obj.OrderId);
            int maxConveyor = await GetMaxConveyor(obj.ConveyorId);
            int remainingQuantity = orderQuantity - totalQuantityOfOrderTask;

            if (conveyor.IsWorking)
            {
                throw new BaseException.BadRequestException("Bad Request", "Băng chuyền này đang hoạt động");
            }

            if (!checkQuantity)
            {
                throw new BaseException.BadRequestException("Bad Request", "Số lượng đơn hàng ít nhất là 1");
            }

            if (checkConveyor)
            {
                throw new BaseException.BadRequestException("Bad Request", $"Số lượng đơn hàng vượt quá số lượng tối đa của băng chuyền (Tối đa: {maxConveyor})");
            }

            if (!checkOrder)
            {
                throw new BaseException.BadRequestException("Bad Request", $"Số lượng yêu cầu vượt quá số lượng đơn hàng! Còn ({orderQuantity} chưa xử lí)");
            }

            if (obj.Quantity > remainingQuantity)
            {
                throw new BaseException.BadRequestException("Bad Request", $"Số lượng yêu cầu vượt quá số lượng đơn hàng! Còn ({remainingQuantity} chưa xử lí)");
            }
        }
        #endregion

    }
}
