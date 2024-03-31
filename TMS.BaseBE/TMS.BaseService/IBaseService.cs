namespace TMS.BaseService
{
    public interface IBaseService<TEntityDto>
    {
        Task<Guid?> CreateAsync(TEntityDto t);
        Task<TEntityDto?> GetByIdAsync(Guid id);

        Task<(IEnumerable<TEntityDto>, int total)> FilterAsync(int skip, int take, string keySearch, IEnumerable<string> filterColumns);

        Task<bool> UpdateAsync(TEntityDto t);

        Task<int> DeleteMultipleAsync(List<string> entityIdList);

        TEntityDto GetNew();

    }
}
