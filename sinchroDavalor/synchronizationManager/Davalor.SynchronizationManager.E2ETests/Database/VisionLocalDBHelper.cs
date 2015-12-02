using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class VisionLocalDBHelper
    {
        readonly string _dbConnectionString;

        string FilePath(string fileName)
        {
            return String.Format("{0}\\{1}\\{2}.txt", System.AppDomain.CurrentDomain.BaseDirectory, "Database\\VisionLocal", fileName);
        }
        public VisionLocalDBHelper(string DbConnectionString)
        {
            _dbConnectionString = DbConnectionString;
        }

        async Task Execute(string command)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_dbConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = command;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
        public async Task CreateDataBase()
        {
            var filePath = FilePath("CreateDatabase");
            using (var reader = new StreamReader(filePath))
            {
                var commandText = await reader.ReadToEndAsync();
                await Execute(commandText);
            }
        }
        public async Task DropDataBase()
        {
            var filePath = FilePath("DropDatabase");
            using (var reader = new StreamReader(filePath))
            {
                var commandText = await reader.ReadToEndAsync();
                await Execute(commandText);
            }
        }
        public async Task CreateTables()
        {
            var filePath = FilePath("CreateTables");
            using (var reader = new StreamReader(filePath))
            {
                var commandText = await reader.ReadToEndAsync();
                await Execute(commandText);
            }
        }
        public async Task DropTables()
        {
            var filePath = FilePath("DropTables");
            using (var reader = new StreamReader(filePath))
            {
                var commandText = await reader.ReadToEndAsync();
                await Execute(commandText);
            }
        }
    }
}
