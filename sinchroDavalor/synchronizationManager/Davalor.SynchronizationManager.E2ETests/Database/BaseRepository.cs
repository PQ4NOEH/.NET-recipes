using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public abstract class BaseRepository<T> where T:class
    {
         readonly protected string _dbConnectionString;
         public BaseRepository(string DbConnectionString)
        {
            _dbConnectionString = DbConnectionString;
        }
        public T Query(string query, Func<SqlDataReader, T> loadTuple)
        {
            using (SqlConnection connection = new SqlConnection(_dbConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows) return null;
                        else
                        {
                            reader.Read();
                            return loadTuple(reader);
                            
                        }
                    }
                }
            }
        }
    }
}
