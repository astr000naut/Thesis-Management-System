using TMS.BaseRepository.Param;

namespace TMS.BaseService
{
    public interface IBaseService<TEntityDto>
    {
        Task<bool?> CreateAsync(TEntityDto t);
        Task<TEntityDto?> GetByIdAsync(string id);

        Task<(IEnumerable<TEntityDto>, int total)> FilterAsync(FilterParam param);

        Task<bool> UpdateAsync(TEntityDto t);

        Task<int> DeleteMultipleAsync(List<string> entityIdList);
        Task<int> DeleteAsync(string id);

        Task<TEntityDto> GetNew();

    }
}
