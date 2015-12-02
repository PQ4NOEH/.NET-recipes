using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.E2ETests.Database
{
    public class PortalPacienteDBHelper
    {
         readonly string _dbConnectionString;

        string FilePath(string fileName)
        {
            return String.Format("{0}\\{1}\\{2}.txt", System.AppDomain.CurrentDomain.BaseDirectory, "Database\\PortalPaciente", fileName);
        }
        public PortalPacienteDBHelper(string DbConnectionString)
        {
            _dbConnectionString = DbConnectionString;
        }

        async Task Execute(string[] commands)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_dbConnectionString))
                {
                    connection.Open();
                    foreach(var command in commands.Where(c=> !string.IsNullOrWhiteSpace(c)))
                    {
                        using (OracleCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = command;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    connection.Close();
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
            var commands = File.ReadAllLines(filePath);
            await Execute(commands);
        }
        public async Task DropDataBase()
        {
            var filePath = FilePath("DropDatabase");
            var commands = File.ReadAllLines(filePath);
            await Execute(commands);
            if(File.Exists(@"C:\Temp\portalPacienteTests.dbf")) File.Delete(@"C:\Temp\portalPacienteTests.dbf");
        }
        public async Task CreateTables()
        {
            var filePath = FilePath("CreateTables");
            using (var reader = new StreamReader(filePath))
            {
                var commandText = await reader.ReadToEndAsync();
                await Execute(commandText.Split(";".ToCharArray()));
            }
        }
    }
}
