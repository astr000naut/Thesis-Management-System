

using System.Data.Common;
using System.Data;
using Dapper;
using TMS.Common.Attribute;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


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

            var properties = typeof(T).GetProperties().Where(
                p => p.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>() == null);

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            var result = await _unitOfWork.Connection.ExecuteAsync(query, t, _unitOfWork.Transaction);

            return result > 0;
        }

        public async Task<int> DeleteMultipleAsync(List<string> listId)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var key = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null).Name;
            var query = $"DELETE FROM {tableName} WHERE {key} IN @listId";
            var deleted = await _unitOfWork.Connection.ExecuteAsync(query, new { listId }, _unitOfWork.Transaction);
            return deleted;
        }

        public async Task<(IEnumerable<T> data, int total)> FilterAsync(int? skip, int? take, string keySearch, IEnumerable<string> filterColumns)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var viewName = (typeof(T).GetCustomAttribute<TableAttribute>()?.ViewName);
            var source = tableName;
            if (!string.IsNullOrEmpty(viewName)) source = viewName;

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
                    FROM {source}
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
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var viewName = (typeof(T).GetCustomAttribute<TableAttribute>()?.ViewName);
            var source = tableName;
            if (!string.IsNullOrEmpty(viewName)) source = viewName;
            var key = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null).Name;
            var query = $"SELECT * FROM {source} WHERE {key} = @id";
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<T>(query, new { id }, _unitOfWork.Transaction);
            return result;
        }

        public async Task<bool> UpdateAsync(T t)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var key = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null).Name;
            var properties = typeof(T).GetProperties().Where(
                p => p.GetCustomAttribute<KeyAttribute>() == null && 
                p.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>() == null);
            var columns = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            var query = $"UPDATE {tableName} SET {columns} WHERE {key} = @{key}";
            var result = await _unitOfWork.Connection.ExecuteAsync(query, t, _unitOfWork.Transaction);
            return result > 0;
        }


    }
}
