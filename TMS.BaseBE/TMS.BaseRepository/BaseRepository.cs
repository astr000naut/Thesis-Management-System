

using System.Data.Common;
using System.Data;
using Dapper;
using TMS.Common.Attribute;
using System.Reflection;

namespace TMS.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T>
    {

        protected readonly IUnitOfWork _unitOfWork;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateAsync(T t)
        {
           
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            var result = await _unitOfWork.Connection.ExecuteAsync(query, t, _unitOfWork.Transaction);

            return result > 0;
        }

        Task<int> DeleteMultipleAsync(List<string> listId)
        {
            throw new NotImplementedException();
        }

        public async Task<(IEnumerable<T> data, int total)> FilterAsync(int? skip, int? take, string keySearch, IEnumerable<string> filterColumns)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);

            string filterWhere = string.Empty;

            if (!string.IsNullOrEmpty(keySearch))
            {
                filterWhere = string.Join(" OR ", filterColumns.Select(column => $"{column} LIKE @keySearch"));
            } else
            {
                filterWhere = "1 = 1";
            }
            
            string sql = $@"
                    SELECT SQL_CALC_FOUND_ROWS *
                    FROM {tableName}
                    WHERE {filterWhere}
                    LIMIT @skip, @take;
                    SELECT FOUND_ROWS() AS Total;";

            
            var multi = await _unitOfWork.Connection.QueryMultipleAsync(sql, new { skip, take, keySearch = $"%{keySearch}%" }, _unitOfWork.Transaction);
            
            var entities = await multi.ReadAsync<T>();

            int total = await multi.ReadFirstOrDefaultAsync<int>();

            return (entities, total);

        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<bool> IBaseRepository<T>.UpdateAsync(T t)
        {
            throw new NotImplementedException();
        }

        Task<int> IBaseRepository<T>.DeleteMultipleAsync(List<string> listId)
        {
            throw new NotImplementedException();
        }

    }
}
