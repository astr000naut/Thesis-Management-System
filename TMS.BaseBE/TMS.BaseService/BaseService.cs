using AutoMapper;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        private readonly string _requestDomain;

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _baseRepository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _requestDomain = _httpContextAccessor.HttpContext.Request.Headers["Origin"]
                                                                     .FirstOrDefault()?.Split("://")[1];
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public virtual void BeforeUpdate() { }  

        public virtual void AfterUpdate() { }

        public async Task<Guid?> CreateAsync(TEntityDto t)
        {
            try
            {
                BeforeCreate();

                await _unitOfWork.OpenAsync();
                var entityInput = _mapper.Map<TEntity>(t);

                Type type = typeof(TEntity);
                var entityName = typeof(TEntity).Name;
                var newId = Guid.NewGuid();

                var idProperty = type.GetProperty($"{entityName}Id");
                idProperty?.SetValue(entityInput, newId);


                await _baseRepository.CreateAsync(entityInput);

                await _unitOfWork.CommitAsync();

                AfterCreate();

                return newId;

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

        public Task<TEntityDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(TEntityDto t)
        {
            throw new NotImplementedException();
        }

        public TEntityDto GetNew()
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
