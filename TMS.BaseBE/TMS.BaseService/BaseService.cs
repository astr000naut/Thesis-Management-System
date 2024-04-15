﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;

namespace TMS.BaseService
{
    public abstract class BaseService<TEntity, TEntityDto> : IBaseService<TEntityDto>
    {

        protected readonly IBaseRepository<TEntity> _baseRepository;
        private IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _baseRepository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async virtual Task BeforeCreate() { }

        public async virtual Task AfterCreate() { }

        public async virtual Task BeforeUpdate(TEntityDto t) { }

        public async virtual Task AfterUpdate() { }

        public async Task<bool?> CreateAsync(TEntityDto t)
        {
            try
            {
                BeforeCreate();

                await _unitOfWork.OpenAsync();
                var entityInput = _mapper.Map<TEntity>(t);


                await _baseRepository.CreateAsync(entityInput);

                await _unitOfWork.CommitAsync();

                AfterCreate();

                return true;

            } catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }    
        }

        public async Task<int> DeleteMultipleAsync(List<string> entityIdList)
        {
            var result = await _baseRepository.DeleteMultipleAsync(entityIdList);
            return result;
        }

        public async Task<(IEnumerable<TEntityDto>, int total)> FilterAsync(int skip, int take, string keySearch, IEnumerable<string> filterColumns)
        {
            try
            {
                await _unitOfWork.OpenAsync();

                var result = await _baseRepository.FilterAsync(skip, take, keySearch, filterColumns);
                var (data, total) = result;
                var dataDto = _mapper.Map<IEnumerable<TEntityDto>>(data);
                await _unitOfWork.CommitAsync();

                return (dataDto, total);
            } catch
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }
        }

        public async Task<TEntityDto?> GetByIdAsync(Guid id)
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            var response = _mapper.Map<TEntityDto>(entity);
            return response;
        }

        public async Task<bool> UpdateAsync(TEntityDto t)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                await BeforeUpdate(t);
                var entity = _mapper.Map<TEntity>(t);
                var result = await _baseRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

            
        }

        public virtual async Task<TEntityDto> GetNew()
        {
            var entity = Activator.CreateInstance<TEntity>();
            var entityDto = _mapper.Map<TEntityDto>(entity);
            entityDto.GetType().GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute))).ToList()
                .ForEach(p => p.SetValue(entityDto, Guid.NewGuid()));

            return entityDto;


        }
    }   
}
