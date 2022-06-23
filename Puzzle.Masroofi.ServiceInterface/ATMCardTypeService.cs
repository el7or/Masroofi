using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ATMCardTypes;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IATMCardTypeService
    {
        Task<PagedOutput<ATMCardTypeOutputViewModel>> GetAllAsync(ATMCardTypeFilterViewModel model);
        Task<ATMCardTypeOutputViewModel> GetAsync(Guid id);
        Task AddAsync(ATMCardTypeInputViewModel model);
        Task UpdateAsync(ATMCardTypeInputViewModel model);
        Task<OperationState> DeleteAsync(Guid id);
    }
    public class ATMCardTypeService : BaseService, IATMCardTypeService
    {
        private readonly IATMCardTypeRepository _atmCardTypeRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IS3Service _s3Service;
        public ATMCardTypeService(IATMCardTypeRepository atmCardTypeRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IS3Service s3Service) : base(unitOfWork, mapper)
        {
            _atmCardTypeRepository = atmCardTypeRepository;
            _userIdentity = userIdentity;
            _s3Service = s3Service;
        }

        public async Task<PagedOutput<ATMCardTypeOutputViewModel>> GetAllAsync(ATMCardTypeFilterViewModel model)
        {
            var result = new PagedOutput<ATMCardTypeOutputViewModel>();

            var query = _atmCardTypeRepository.Table;

            // filtering
            query = FilterATMCardTypes(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<ATMCardType, object>>>()
            {
                ["typeName"] = v => _userIdentity.Language == Language.en ? v.TypeNameEn : v.TypeNameAr,
                ["cost"] = v => v.Cost,
                ["creationDate"] = v => v.CreationDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<ATMCardTypeOutputViewModel>>(query);

            return result;
        }

        public async Task<ATMCardTypeOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _atmCardTypeRepository.GetAsync(id);

            if (entityToGet == null)
            {
                throw new BusinessException("ATMCardType Not found!");
            }
            return mapper.Map<ATMCardTypeOutputViewModel>(entityToGet);
        }

        public async Task AddAsync(ATMCardTypeInputViewModel model)
        {
            model.ATMCardTypeId = Guid.NewGuid();

            var errors = new List<Exception>();

            if (model.NewFrontImage == null || string.IsNullOrEmpty(model.NewFrontImage.FileBase64))
                throw new BusinessException("NewFrontImage is required!");

            if (model.NewBackImage == null || string.IsNullOrEmpty(model.NewBackImage.FileBase64))
                throw new BusinessException("NewBackImage is required!");

            ValidateATMCardType(model, errors);

            var entityToAdd = mapper.Map<ATMCardType>(model);

            if (model.NewFrontImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewFrontImage.FileBase64);
                var imagePath = _s3Service.UploadFile("ATMCardTypes", model.NewFrontImage.FileName, fileBytes);
                entityToAdd.FrontImageUrl = imagePath;
            }

            if (model.NewBackImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewBackImage.FileBase64);
                var imagePath = _s3Service.UploadFile("ATMCardTypes", model.NewBackImage.FileName, fileBytes);
                entityToAdd.BackImageUrl = imagePath;
            }

            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = _userIdentity.Id.Value;
            _atmCardTypeRepository.Add(entityToAdd);

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToAdd, model);
            }
            else
                throw new BusinessException("Unable to add ATMCardType!");
        }

        public async Task UpdateAsync(ATMCardTypeInputViewModel model)
        {
            var errors = new List<Exception>();

            if ((model.NewFrontImage == null || string.IsNullOrEmpty(model.NewFrontImage.FileBase64)) && string.IsNullOrEmpty(model.FrontImageUrl))
                throw new BusinessException("NewFrontImage or old FrontImageUrl is required!");

            if ((model.NewBackImage == null || string.IsNullOrEmpty(model.NewBackImage.FileBase64)) && string.IsNullOrEmpty(model.BackImageUrl))
                throw new BusinessException("NewBackImage or old BackImageUrl is required!");

            ValidateATMCardType(model, errors);

            var entityToUpdate = await _atmCardTypeRepository.GetAsync(model.ATMCardTypeId.Value);

            if (entityToUpdate == null)
            {
                throw new BusinessException("ATMCardType Not found!");
            }

            mapper.Map(model, entityToUpdate);

            if (model.NewFrontImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewFrontImage.FileBase64);
                var imagePath = _s3Service.UploadFile("ATMCardTypes", model.NewFrontImage.FileName, fileBytes);
                entityToUpdate.FrontImageUrl = imagePath;
            }

            if (model.NewBackImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewBackImage.FileBase64);
                var imagePath = _s3Service.UploadFile("ATMCardTypes", model.NewBackImage.FileName, fileBytes);
                entityToUpdate.BackImageUrl = imagePath;
            }

            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
            }
            else
                throw new BusinessException("Unable to update ATMCardType!");
        }

        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _atmCardTypeRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("ATMCardType Not found!");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        private IQueryable<ATMCardType> FilterATMCardTypes(IQueryable<ATMCardType> query, ATMCardTypeFilterViewModel model)
        {
            if (!string.IsNullOrEmpty(model.TypeName))
            {
                query = query.Where(c => _userIdentity.Language == Language.en ? c.TypeNameEn.Contains(model.TypeName) :
                _userIdentity.Language == Language.ar ? c.TypeNameAr.Contains(model.TypeName) :
                c.TypeNameAr.Contains(model.TypeName) || c.TypeNameEn.Contains(model.TypeName));
            }
            if (model.Cost.HasValue)
            {
                query = query.Where(c => c.Cost == model.Cost);
            }

            return query;
        }

        private void ValidateATMCardType(ATMCardTypeInputViewModel model, List<Exception> errors)
        {
            if (model.ATMCardTypeId == null || model.ATMCardTypeId == Guid.Empty)
                errors.Add(new Exception("ATMCardTypeId is required!"));

            if (string.IsNullOrEmpty(model.TypeNameAr))
                errors.Add(new Exception("TypeNameAr is required!"));

            if (string.IsNullOrEmpty(model.TypeNameEn))
                errors.Add(new Exception("TypeNameEn is required!"));

            if (!(model.Cost >= 0))
                errors.Add(new Exception("Cost is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}
