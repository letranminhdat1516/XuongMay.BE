﻿using Azure.Core;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Base;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.ConveyorModelViews;

namespace XuongMay.Services.Service
{
    public class ConveyorService : IConveyorService
    {
        #region Attribute
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Conveyor> _conveyorRepository;

        #endregion

        #region Contructor
        public ConveyorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _conveyorRepository = unitOfWork.GetRepository<Conveyor>();
        }
        #endregion

        #region Get all Conveyor with paging
        public Task<BasePaginatedList<Conveyor>> GetAllConveyorPaging(int index, int pageSize)
        {
            var conveyors = _conveyorRepository.Entities.Where(con => con.DeletedTime == null);
            var conveyorsPaging = _conveyorRepository.GetPagging(conveyors, index, pageSize);
            return conveyorsPaging;
        }
        #endregion

        #region Get one Conveyor
        public Task<Conveyor?> GetOneConveyor(object id)
        {
            var conveyor = _conveyorRepository.GetById(id);
            return Task.FromResult(conveyor);
        }
        #endregion

        #region Insert Convenyor
        public async Task InsertNewConveyor(ConveyorRequestModel obj)
        {
            Conveyor conveyor = new Conveyor();
            conveyor.ConveyorName = obj.ConveyorName;
            conveyor.ConveyorCode = obj.ConveyorCode;
            conveyor.ConveyorNumber = obj.ConveyorNumber;
            conveyor.MaxQuantity = obj.MaxQuantity;
            conveyor.CreatedBy = obj.CreateBy;
            await _conveyorRepository.InsertAsync(conveyor);
            await SaveAsync();
        }
        #endregion

        #region Update Convenyor
        public async Task UpdateConveyor(ConveyorUpdateModel obj)
        {
            var conveyor = _conveyorRepository.GetById(obj.ConveyorId);
            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }

            if (conveyor.IsWorking)
            {
                throw new BaseException.BadRequestException("Bad Request", "Không thể cập nhật! Băng chuyền đang hoạt động");
            }

            conveyor.ConveyorNumber = obj.ConveyorNumber;
            conveyor.ConveyorName = obj.ConveyorName;
            conveyor.ConveyorCode = obj.ConveyorCode;
            conveyor.MaxQuantity = obj.MaxQuantity;
            conveyor.LastUpdatedTime = CoreHelper.SystemTimeNow;
            conveyor.LastUpdatedBy = obj.UpdateBy;

            await _conveyorRepository.UpdateAsync(conveyor);
            await SaveAsync();
        }
        #endregion

        #region Delete Conveyor
        public async Task DeleteConveyor(object id, string deleteBy)
        {
            var conveyor = _conveyorRepository.GetById(id);
            if (conveyor == null)
            {
                throw new BaseException.ErrorException(404, "Not Found", "Không tìm thấy băng chuyền");
            }
            if (conveyor.IsWorking)
            {
                throw new BaseException.BadRequestException("Bad Request", "Không thể xóa! Băng chuyền đang hoạt động");
            }

            conveyor.DeletedTime = CoreHelper.SystemTimeNow;
            conveyor.DeletedBy = deleteBy;
            await _conveyorRepository.UpdateAsync(conveyor);
            //await _conveyorRepository.DeleteAsync(id);
            await SaveAsync();
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
