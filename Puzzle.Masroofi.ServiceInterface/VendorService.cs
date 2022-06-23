using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Vendors;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IVendorService
    {
        Task<PagedOutput<VendorOutputViewModel>> GetAllAsync(VendorFilterViewModel model);
        Task<VendorOutputViewModel> GetAsync(Guid id);
        Task AddAsync(VendorInputViewModel model);
        Task UpdateAsync(VendorInputViewModel model);
        Task<OperationState> DeleteAsync(Guid id);
        Task<decimal> GetBalanceAsync(Guid id);
    }
    public class VendorService : BaseService, IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IS3Service _s3Service;
        public VendorService(IVendorRepository vendorRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IS3Service s3Service) : base(unitOfWork, mapper)
        {
            _vendorRepository = vendorRepository;
            _userIdentity = userIdentity;
            _s3Service = s3Service;
        }

        public async Task<PagedOutput<VendorOutputViewModel>> GetAllAsync(VendorFilterViewModel model)
        {
            var result = new PagedOutput<VendorOutputViewModel>();

            var query = _vendorRepository.Table;

            // filtering
            query = FilterVendors(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<Vendor, object>>>()
            {
                ["fullName"] = v => _userIdentity.Language == Language.en ? v.FullNameEn : v.FullNameAr,
                ["cityId"] = v => v.CityId,
                ["currentBalance"] = v => v.CurrentBalance,
                ["creationDate"] = v => v.CreationDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<VendorOutputViewModel>>(query);

            return result;
        }

        public async Task<VendorOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _vendorRepository.GetAsync(id);

            if (entityToGet == null)
            {
                throw new BusinessException("Vendor Not found!");
            }
            return mapper.Map<VendorOutputViewModel>(entityToGet);
        }

        public async Task AddAsync(VendorInputViewModel model)
        {
            model.VendorId = Guid.NewGuid();

            ValidateVendor(model);

            var entityToAdd = mapper.Map<Vendor>(model);

            if (model.NewImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewImage.FileBase64);
                var imagePath = _s3Service.UploadFile("Vendors", model.NewImage.FileName, fileBytes);
                entityToAdd.ImageUrl = imagePath;
            }

            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = _userIdentity.Id.Value;
            _vendorRepository.Add(entityToAdd);

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToAdd, model);
            }
            else
                throw new BusinessException("Unable to add Vendor!");
        }

        public async Task UpdateAsync(VendorInputViewModel model)
        {
            ValidateVendor(model);

            var entityToUpdate = await _vendorRepository.GetAsync(model.VendorId.Value);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Vendor Not found!");
            }

            mapper.Map(model, entityToUpdate);

            if (model.NewImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewImage.FileBase64);
                var imagePath = _s3Service.UploadFile("Vendors", model.NewImage.FileName, fileBytes);
                entityToUpdate.ImageUrl = imagePath;
            }

            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
            }
            else
                throw new BusinessException("Unable to update Vendor!");
        }

        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _vendorRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("Vendor Not found!");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<decimal> GetBalanceAsync(Guid id)
        {
            var vendor = await _vendorRepository.GetAsync(id);
            if (vendor == null)
            {
                throw new BusinessException("Vendor Not found!");
            }
            return vendor.CurrentBalance;
        }

        private IQueryable<Vendor> FilterVendors(IQueryable<Vendor> query, VendorFilterViewModel model)
        {
            if (!string.IsNullOrEmpty(model.FullName))
            {
                query = query.Where(c => _userIdentity.Language == Language.en ? c.FullNameEn.Contains(model.FullName) :
                _userIdentity.Language == Language.ar ? c.FullNameAr.Contains(model.FullName) :
                c.FullNameAr.Contains(model.FullName) || c.FullNameEn.Contains(model.FullName));
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                query = query.Where(c => c.Phone1.Contains(model.Phone) || c.Phone2.Contains(model.Phone));
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                query = query.Where(c => c.Email.Contains(model.Email));
            }
            if (model.CityId.HasValue)
            {
                query = query.Where(c => c.CityId == model.CityId);
            }

            return query;
        }

        private void ValidateVendor(VendorInputViewModel model)
        {
            var errors = new List<Exception>();

            if (model.VendorId == null || model.VendorId == Guid.Empty)
                errors.Add(new Exception("VendorId is required!"));

            if (string.IsNullOrEmpty(model.FullNameAr))
                errors.Add(new Exception("FullNameAr is required!"));

            if (string.IsNullOrEmpty(model.FullNameEn))
                errors.Add(new Exception("FullNameEn is required!"));

            if (model.CityId < 1)
                errors.Add(new Exception("CityId is required!"));

            if (string.IsNullOrEmpty(model.Phone1))
                errors.Add(new Exception("Phone1 is required!"));

            if (!model.Phone1.IsValidPhoneNumber())
                errors.Add(new Exception("Invalid Phone1 number!"));

            if (!string.IsNullOrEmpty(model.Phone2) && !model.Phone2.IsValidPhoneNumber())
                errors.Add(new Exception("Invalid Phone2 number!"));

            if (!string.IsNullOrEmpty(model.Email) && !model.Email.IsValidEmailAddress())
                errors.Add(new Exception("Invalid Email address!"));

            if (string.IsNullOrEmpty(model.ResponsiblePerson))
                errors.Add(new Exception("ResponsiblePerson is required!"));

            if (string.IsNullOrEmpty(model.Address))
                errors.Add(new Exception("Address is required!"));

            if (string.IsNullOrEmpty(model.GoogleLocation))
                errors.Add(new Exception("GoogleLocation is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}
