using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BaseRepository
{
    public interface IUnitOfWork
    {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }

        void SetConnectionString(string connectionString);
        Task OpenAsync();
        Task CloseAsync();
        Task CommitAsync();
    }
}
