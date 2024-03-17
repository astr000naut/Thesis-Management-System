using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace TMS.BaseRepository
{
    public class UnitOfWork: IUnitOfWork
    {
        private DbConnection _connection;
        private DbTransaction _transaction;
        private int _manipulationKey;

        public UnitOfWork()
        {
            _manipulationKey = 0;
        }


        public DbConnection Connection => _connection;

        public DbTransaction Transaction => _transaction;


        public void SetConnectionString(string connectionString)
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(connectionString);
            }
        }

        public async Task OpenAsync()
        {
            if (_connection == null)
            {
                throw new Exception("Connection is null");
            }

            if (_manipulationKey == 0)
            {
                await _connection.OpenAsync();
                _transaction = await _connection.BeginTransactionAsync();
                _manipulationKey = 1;
            }
        }

        public async Task CloseAsync()
        {
            if (_connection == null)
            {
                throw new Exception("Connection is null");
            }

            if (_manipulationKey == 1)
            {
                if (_transaction != null)
                    await _transaction.DisposeAsync();
                _transaction = null;

                await _connection.CloseAsync();
                _manipulationKey = 0;

            } else
            {
                _manipulationKey -= 1;
            }
        }

        public async Task CommitAsync()
        {
            if (_connection == null)
            {
                throw new Exception("Connection is null");
            }

            if (_manipulationKey == 1)
            {
                await _transaction.CommitAsync();
            }
        }
    }
}
