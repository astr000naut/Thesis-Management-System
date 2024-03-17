namespace TMS.BaseService
{
    public interface IBaseService<TEntityDto>
    {
        Task<Guid?> CreateAsync(TEntityDto t);
        Task<TEntityDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<TEntityDto>> FilterAsync(int skip, int take, int keySearch);

        Task<bool> UpdateAsync(TEntityDto t);

        Task<int> DeleteMultipleAsync(List<Guid> entityIdList);

    }
}
