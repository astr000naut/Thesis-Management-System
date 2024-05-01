using TMS.BaseRepository.Param;

namespace TMS.BaseService
{
    public interface IBaseService<TEntityDto>
    {
        Task<bool?> CreateAsync(TEntityDto t);
        Task<TEntityDto?> GetByIdAsync(Guid id);

        Task<(IEnumerable<TEntityDto>, int total)> FilterAsync(FilterParam param);

        Task<bool> UpdateAsync(TEntityDto t);

        Task<int> DeleteMultipleAsync(List<string> entityIdList);

        Task<TEntityDto> GetNew();

    }
}
