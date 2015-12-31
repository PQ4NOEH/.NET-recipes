// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDatabaseManager.cs" company="ALTEA">
//   No copyright yet.
// </copyright>
// <summary>
//   Static class for querying SQL Server databases.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Altea.Database
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Static class for querying SQL Server databases.
    /// </summary>
    public class SqlDatabaseManager
    {
        /// <summary>
        /// SQL Factory used for creating new commands and parameters.
        /// </summary>
        private static readonly SqlClientFactory Instance = SqlClientFactory.Instance;


        private static bool initialized;

        /// <summary>
        /// Maximum number of retries before aborting queries.
        /// </summary>
        private static int connectionRetries;

        /// <summary>
        /// Number of seconds to wait before first retry of a query.
        /// </summary>
        private static int connectionRetryWaitSeconds;


        /// <summary>
        /// Maximum number of retries before aborting queries.
        /// </summary>
        private readonly int instanceRetries;

        /// <summary>
        /// Number of seconds to wait before first retry of a query.
        /// </summary>
        private readonly int instanceRetryWaitSeconds;

        /// <summary>
        /// Connection string.
        /// </summary>
        private readonly string instanceConnectionString;


        private SqlDatabaseManager(int retries, int waitSeconds, string connectionString)
        {
            this.instanceRetries = retries;
            this.instanceRetryWaitSeconds = waitSeconds;
            this.instanceConnectionString = connectionString;
        }


        public static SqlDatabaseManager Create(int retries, int waitSeconds, string connectionString)
        {
            return new SqlDatabaseManager(retries, waitSeconds, connectionString);
        }


        /// <summary>
        /// Creates a new SQL command, either a full query of an stored procedure
        /// </summary>
        /// <param name="type">
        /// Command type.
        /// </param>
        /// <param name="text">
        /// SQL query or stored procedure name.
        /// </param>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Class provides easier database access methods, including plain SQL queries.")]
        public static SqlCommand CreateCommand(CommandType type, string text)
        {
            SqlCommand command = (SqlCommand)Instance.CreateCommand();
            command.CommandType = type;
            command.CommandText = text;
            return command;
        }

        /// <summary>
        /// Add a parameter to an existing command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="direction">
        /// Parameter direction.
        /// </param>
        /// <param name="type">
        /// Parameter SQL type.
        /// </param>
        /// <returns>
        /// The <see cref="SqlParameter"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Neither Command object nor parameter name can be null.
        /// </exception>
        public static SqlParameter AddParameter(
            SqlCommand command,
            string name,
            ParameterDirection direction,
            SqlDbType type)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            SqlParameter parameter = (SqlParameter)Instance.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            parameter.SqlDbType = type;
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Add a variable sized parameter to an existing command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="direction">
        /// Parameter direction.
        /// </param>
        /// <param name="type">
        /// Parameter SQL type.
        /// </param>
        /// <param name="size">
        /// Parameter size.
        /// </param>
        /// <returns>
        /// The <see cref="SqlParameter"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Neither Command object nor parameter name can be null.
        /// </exception>
        public static SqlParameter AddSizedParameter(
            SqlCommand command,
            string name,
            ParameterDirection direction,
            SqlDbType type,
            int size)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            SqlParameter parameter = (SqlParameter)Instance.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            parameter.SqlDbType = type;
            parameter.Size = size;
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Add a parameter with value to an existing command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="direction">
        /// Parameter direction.
        /// </param>
        /// <param name="type">
        /// Parameter SQL type.
        /// </param>
        /// <param name="value">
        /// Parameter value.
        /// </param>
        /// <returns>
        /// The <see cref="SqlParameter"/>.
        /// </returns>
        public static SqlParameter AddParameter(
            SqlCommand command,
            string name,
            ParameterDirection direction,
            SqlDbType type,
            object value)
        {
            SqlParameter parameter = AddParameter(command, name, direction, type);
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        /// <summary>
        /// Add a parameter with value and user-defined type to an existing command. Used with Structured types.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="name">
        /// Parameter name.
        /// </param>
        /// <param name="direction">
        /// Parameter direction.
        /// </param>
        /// <param name="typeName">
        /// Parameter user-defined SQL type.
        /// </param>
        /// <param name="value">
        /// Parameter value.
        /// </param>
        /// <returns>
        /// The <see cref="SqlParameter"/>.
        /// </returns>
        public static SqlParameter AddParameter(
            SqlCommand command,
            string name,
            ParameterDirection direction,
            string typeName,
            object value)
        {
            SqlParameter parameter = AddParameter(command, name, direction, SqlDbType.Structured, value);
            parameter.TypeName = typeName;
            return parameter;
        }

        #region Static Executes
        /// <summary>
        /// Executes a command with no results.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        public static int ExecuteNonQuery(SqlCommand command, string connectionString)
        {
            return (int)Execute(command, connectionRetries, connectionRetryWaitSeconds, DatabaseSettings.GetConnectionString(connectionString), cmd => cmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes asynchronously a command with no results.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        public static Task<int> ExecuteNonQueryAsync(SqlCommand command, string connectionString)
        {
            return Task.Run(() => ExecuteNonQuery(command, connectionString), CancellationToken.None);
        }

        /// <summary>
        /// Executes a command and returns the first value in the result set.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public static object ExecuteScalar(SqlCommand command, string connectionString)
        {
            return ExecuteScalar<object>(command, connectionString);
        }

        /// <summary>
        /// Executes a command and returns the first value in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the returned value.
        /// </typeparam>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public static T ExecuteScalar<T>(SqlCommand command, string connectionString)
        {
            return (T)Execute(command, connectionRetries, connectionRetryWaitSeconds, DatabaseSettings.GetConnectionString(connectionString), cmd => cmd.ExecuteScalar());
        }

        /// <summary>
        /// Executes asynchronously a command and returns the first value in the result set.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public static Task<object> ExecuteScalarAsync(SqlCommand command, string connectionString)
        {
            return ExecuteScalarAsync<object>(command, connectionString);
        }

        /// <summary>
        /// Executes asynchronously a command and returns the first value in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the returned value.
        /// </typeparam>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public static Task<T> ExecuteScalarAsync<T>(SqlCommand command, string connectionString)
        {
            return Task.Run(() => ExecuteScalar<T>(command, connectionString), CancellationToken.None);
        }

        /// <summary>
        /// Executes a command and perform an action with the reader obtained.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Reader must have an not-null action.
        /// </exception>
        public static int ExecuteReader(SqlCommand command, string connectionString, Action<SqlDataReader> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return (int)Execute(
                command,
                connectionRetries,
                connectionRetryWaitSeconds,
                DatabaseSettings.GetConnectionString(connectionString),
                cmd =>
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        action.Invoke(reader);
                        return reader.RecordsAffected;
                    }
                });
        }
        #endregion

        #region Instance Executes
        /// <summary>
        /// Executes a command with no results.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        public int ExecuteNonQuery(SqlCommand command)
        {
            return (int)Execute(command, this.instanceRetries, this.instanceRetryWaitSeconds, this.instanceConnectionString, cmd => cmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes asynchronously a command with no results.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        public Task<int> ExecuteNonQueryAsync(SqlCommand command)
        {
            return Task.Run(() => this.ExecuteNonQuery(command), CancellationToken.None);
        }

        /// <summary>
        /// Executes a command and returns the first value in the result set.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public object ExecuteScalar(SqlCommand command)
        {
            return this.ExecuteScalar<object>(command);
        }

        /// <summary>
        /// Executes a command and returns the first value in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the returned value.
        /// </typeparam>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public T ExecuteScalar<T>(SqlCommand command)
        {
            return (T)Execute(command, this.instanceRetries, this.instanceRetryWaitSeconds, this.instanceConnectionString, cmd => cmd.ExecuteScalar());
        }

        /// <summary>
        /// Executes asynchronously a command and returns the first value in the result set.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public Task<object> ExecuteScalarAsync(SqlCommand command)
        {
            return this.ExecuteScalarAsync<object>(command);
        }

        /// <summary>
        /// Executes asynchronously a command and returns the first value in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the returned value.
        /// </typeparam>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The value returned by the command.
        /// </returns>
        public Task<T> ExecuteScalarAsync<T>(SqlCommand command)
        {
            return Task.Run(() => this.ExecuteScalar<T>(command), CancellationToken.None);
        }

        /// <summary>
        /// Executes a command and perform an action with the reader obtained.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Reader must have an not-null action.
        /// </exception>
        public int ExecuteReader(SqlCommand command, Action<SqlDataReader> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return (int)Execute(
                command,
                this.instanceRetries,
                this.instanceRetryWaitSeconds,
                this.instanceConnectionString,
                cmd =>
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        action.Invoke(reader);
                        return reader.RecordsAffected;
                    }
                });
        }
        #endregion

        /// <summary>
        /// Initializes the static class of the manager.
        /// </summary>
        private static void Initialize()
        {
            connectionRetries = DatabaseSettings.ConnectionRetries;
            connectionRetryWaitSeconds = DatabaseSettings.ConnectionRetryWaitSeconds;
            initialized = true;
        }

        /// <summary>
        /// Calculates the number of seconds to wait between query retries.
        /// </summary>
        /// <param name="attempt">
        /// The attempt number.
        /// </param>
        /// <param name="waitSeconds">
        /// The default number of seconds to wait.
        /// </param>
        /// <returns>
        /// The number of seconds to wait.
        /// </returns>
        private static int GetConnectionRetryWaitSeconds(int attempt, int waitSeconds)
        {
            // Backoff Throttling
            // http://blogs.msdn.com/b/sqlazure/archive/2010/05/11/10011247.aspx
            return waitSeconds * (int)Math.Pow(2, attempt);
        }

        /// <summary>
        /// Determine from the exception if the execution of the connection should be attempted again.
        /// </summary>
        /// <param name="sqlException">
        /// A SQL generic exception
        /// </param>
        /// <returns>
        /// True if a retry is needed, false if not
        /// </returns>
        private static bool IsTransientException(SqlException sqlException)
        {
            switch (sqlException.Number)
            {
                // The service has encountered an error
                // processing your request. Please try again.
                // Error code %d.
                case 40197:
                // The service is currently busy. Retry
                // the request after 10 seconds. Code: %d.
                case 40501:
                // A transport-level error has occurred when
                // receiving results from the server. (provider:
                // TCP Provider, error: 0 - An established connection
                // was aborted by the software in your host machine.)
                case 10053:

                // From: System.Data.Entity.SqlServer.SqlAzureExecutionStrategy
                // From: http://www.getcodesamples.com/src/A122E27E/5586EA7F

                // SQL Error Code: 10054
                // A transport-level error has occurred when
                // sending the request to the server. (provider:
                // TCP Provider, error: 0 - An existing connection
                // was forcibly closed by the remote host.)
                case 10054:
                // SQL Error Code: 10060
                // A network-related or instance-specific error occurred
                // while establishing a connection to SQL Server. The
                // server was not found or was not accessible. Verify
                // that the instance name is correct and that SQL Server
                // is configured to allow remote connections. (provider:
                // TCP Provider, error: 0 - A connection attempt failed
                // because the connected party did not properly respond
                // after a period of time, or established connection failed
                // because connected host has failed to respond.)"}
                case 10060:
                // SQL Error Code: 40613
                // Database XXXX on server YYYY is not currently available.
                // Please retry the connection later. If the problem persists,
                // contact customer support, and provide them the session
                // tracing ID of ZZZZZ.
                case 40613:
                // SQL Error Code: 40143
                // The service has encountered an error processing your
                // request. Please try again.
                case 40143:
                // SQL Error Code: 233
                // The client was unable to establish a connection because of
                // an error during connection initialization process before login.
                // Possible causes include the following: the client tried to
                // connect to an unsupported version of SQL Server; the server was
                // too busy to accept new connections; or there was a resource
                // limitation (insufficient memory or maximum allowed connections) on
                // the server. (provider: TCP Provider, error: 0 - An existing
                // connection was forcibly closed by the remote host.)
                case 233:
                // SQL Error Code: 64
                // A connection was successfully established with the server, but
                // then an error occurred during the login process. (provider: TCP
                // Provider, error: 0 - The specified network name is no longer available.)
                case 64:
                // SQL Error Code: 20
                // The instance of SQL Server you attempted to connect to does not support encryption.
                case 20:

                // From: http://blogs.msdn.com/b/psssql/archive/2012/10/31/worker-thread-governance-coming-to-azure-sql-database.aspx

                // SQL Error Code: 10928
                // Resource ID: %d. The %s limit for the database is %d and has been
                // reached. See http://go.microsoft.com/fwlink/?LinkId=267637 This link
                // is external to TechNet Wiki. It will open in a new window for assistance. 
                case 10928:
                // SQL Error Code: 10929
                // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and
                // the current usage for the database is %d. However, the server is currently
                // too busy to support requests greater than %d for this database. See
                // http://go.microsoft.com/fwlink/?LinkId=267637 This link is external to TechNet
                // Wiki. It will open in a new window. for assistance. Otherwise, please try again later.
                case 10929:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates a new SQL Server connection.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="SqlConnection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// A not-null connection string must be provided.
        /// </exception>
        private static SqlConnection CreateConnection(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            SqlConnection connection = (SqlConnection)Instance.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Executes a command with a defined action.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="retries">
        /// The number of transient retries.
        /// </param>
        /// <param name="waitSeconds">
        /// The seconds to wait between retries.
        /// </param>
        /// <param name="connectionString">
        /// The database connection string against the command will be executed.
        /// </param>
        /// <param name="action">
        /// The action to execute.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An existing not-null command must be provided.
        /// </exception>
        private static object Execute(SqlCommand command, int retries, int waitSeconds, string connectionString, Func<SqlCommand, object> action)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (!initialized)
            {
                Initialize();
            }

            for (int attempt = 1;;)
            {
                using (SqlConnection connection = CreateConnection(connectionString))
                {
                    try
                    {
                        command.Connection = connection;
                        return action.Invoke(command);
                    }
                    catch (SqlException sqlException)
                    {
                        if (++attempt == retries || !IsTransientException(sqlException))
                        {
                            throw;
                        }

                        Thread.Sleep(GetConnectionRetryWaitSeconds(attempt, waitSeconds));
                    }
                    catch (TimeoutException)
                    {
                        if (++attempt == connectionRetries)
                        {
                            throw;
                        }

                        Thread.Sleep(GetConnectionRetryWaitSeconds(attempt, waitSeconds));
                    }
                }
            }
        }
    }
}
