

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
            // get table name in the custom attribute of t entity then create a query to insert into that table

            var tableName = (typeof(T).GetCustomAttribute<TableAttribute>()?.TableName);
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            var result = await _unitOfWork.Connection.ExecuteAsync(query, t, _unitOfWork.Transaction);
            return result > 0;
        }

        Task<int> IBaseRepository<T>.DeleteMultipleAsync(List<string> listId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<T>> IBaseRepository<T>.FilterAsync(int? skip, int? take, string keySearch)
        {
            throw new NotImplementedException();
        }

        Task<T?> IBaseRepository<T>.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<bool> IBaseRepository<T>.UpdateAsync(T t)
        {
            throw new NotImplementedException();
        }
    }
}
