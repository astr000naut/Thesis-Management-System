using AutoMapper;
using System;
using System.Collections.Generic;
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
                return null;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }    
        }

        public Task<int> DeleteMultipleAsync(List<Guid> entityIdList)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntityDto>> FilterAsync(int skip, int take, int keySearch)
        {
            throw new NotImplementedException();
        }

        public Task<TEntityDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(TEntityDto t)
        {
            throw new NotImplementedException();
        }
    }   
}
