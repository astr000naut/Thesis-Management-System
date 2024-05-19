

using System.Data.Common;
using System.Data;
using Dapper;
using TMS.Common.Attribute;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using TMS.BaseRepository.Param;


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

        public async Task<(IEnumerable<T> data, int total)> FilterAsync(FilterParam fp)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var viewName = (typeof(T).GetCustomAttribute<TableAttribute>()?.ViewName);
            var source = tableName;
            if (!string.IsNullOrEmpty(viewName)) source = viewName;

            string filterWhere = string.Empty;

            if (!string.IsNullOrEmpty(fp.KeySearch) && fp.FilterColumns != null)
            {
                filterWhere = string.Join(" OR ", fp.FilterColumns.Select(column => $"{column} LIKE @keySearch"));
                filterWhere = "(" + filterWhere + ")";
            } else
            {
                filterWhere = "1 = 1";
            }
            string customWhere = string.Empty;
            Dictionary<string, string> dicParam = null;

            if (fp.CustomWhere != null && fp.CustomWhere.Count > 0)
            {
                (customWhere, dicParam) = GenerateCustomWhere(fp.CustomWhere);
            }

            string skipTakeQuery = fp.Take > 0 ? "LIMIT @skip, @take" : "" ;
            
            string sql = $@"
                    SELECT SQL_CALC_FOUND_ROWS *
                    FROM {source}
                    WHERE {filterWhere}
                    {customWhere}
                    {skipTakeQuery};
                    SELECT FOUND_ROWS() AS Total;";

            var param = new DynamicParameters();
            param.Add("@skip", fp.Skip);
            param.Add("@take", fp.Take);
            param.Add("@keySearch", $"%{fp.KeySearch}%");

            if (dicParam != null)
            {
                foreach (var item in dicParam)
                {
                    param.Add(item.Key, item.Value);
                }
            }   

            var multi = await _unitOfWork.Connection.QueryMultipleAsync(sql, param, _unitOfWork.Transaction);
            
            var entities = await multi.ReadAsync<T>();

            int total = await multi.ReadFirstOrDefaultAsync<int>();

            return (entities, total);

        }

        public (string, Dictionary<string, string>) GenerateCustomWhere(List<CustomWhere> customWhere)
        {
            string res = string.Empty;
            // handle sql injection here

            var dicParam = new Dictionary<string, string>();

            foreach (var cw in customWhere)
            {
                // check cw.Command is valid
                if (cw.Command != "AND" && cw.Command != "OR")
                {
                    continue;
                }

                //check cw.Operator is valid
                if (cw.Operator != "=" && cw.Operator != "!=" && cw.Operator != ">" && cw.Operator != "<" && cw.Operator != ">=" && cw.Operator != "<=")
                {
                    continue;
                }

                // check cw.ColumnName is valid
                var properties = typeof(T).GetProperties().Select(p => p.Name.ToLower());
                if (!properties.Contains(cw.ColumnName.ToLower()))
                {
                    continue;
                }

                var key = $"@p{Guid.NewGuid().ToString().Substring(0, 4)}";
                while (dicParam.ContainsKey(key))
                {
                    key = $"@p{Guid.NewGuid().ToString().Substring(0, 4)}";
                }

                res += $" {cw.Command} {cw.ColumnName} {cw.Operator} {key}";
                dicParam.Add(key, cw.Value);
            }
            return (res, dicParam);

        }

        public async Task<int> DeleteAsync(string id)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var key = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null).Name;
            var query = $"DELETE FROM {tableName} WHERE {key} = @id";
            var result = await _unitOfWork.Connection.ExecuteAsync(query, new { id }, _unitOfWork.Transaction);
            return result;
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var viewName = (typeof(T).GetCustomAttribute<TableAttribute>()?.ViewName);
            var hasDetail = (typeof(T).GetCustomAttribute<TableAttribute>()?.HasDetail);
            var source = tableName;
            if (!string.IsNullOrEmpty(viewName)) source = viewName;
            var key = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null).Name;
            var query = $"SELECT * FROM {source} WHERE {key} = @id";
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<T>(query, new { id }, _unitOfWork.Transaction);
            if (result != null && hasDetail == true)
            {
                result = await GetDetailData(result);
            }
            return result;
        }

        public virtual Task<T> GetDetailData(T t)
        {
            throw new NotImplementedException();
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
