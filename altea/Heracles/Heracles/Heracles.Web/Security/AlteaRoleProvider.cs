namespace Heracles.Web.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web.Security;

    using Altea.Database;
    using Altea.Extensions;

    public class AlteaRoleProvider : RoleProvider
    {
        private string _appName;

        public override string ApplicationName
        {
            get
            {
                return this._appName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("ApplicationName");
                }

                if (value.Length > 256)
                {
                    throw new ArgumentException("Application name is too long", "ApplicationName");
                }

                this._appName = value;
            }
        }



        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "AlteaSqlMembershipProvider";
            }

            base.Initialize(name, config);

            this._appName = config["applicationName"];
            if (string.IsNullOrEmpty(this._appName))
            {
                throw new ArgumentNullException("ApplicationName");
            }

            if (this._appName.Length > 256)
            {
                throw new ArgumentException("Application name is too long", "ApplicationName");
            }

            config.Remove("applicationName");

            if (config.Count <= 0)
            {
                return;
            }

            string key = config.GetKey(0);
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            throw new ArgumentException("Unrecognized attribute", key);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInRoles_IsUserInRole]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@rolename",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    roleName);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);

                switch ((AlteaProviderStatusCode)command.Parameters["@status"].Value)
                {
                    case AlteaProviderStatusCode.False:
                        return false;

                    case AlteaProviderStatusCode.True:
                        return true;

                    case AlteaProviderStatusCode.InvalidUser:
                    case AlteaProviderStatusCode.InvalidData:
                    case AlteaProviderStatusCode.UnknownError:
                        return false;

                    default:
                        throw new ProviderException("Unknown error");
                }
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            List<string> rolenames = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInRoles_GetRolesForUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        rolenames = new List<string>();

                        while (reader.Read())
                        {
                            rolenames.Add((string)reader["RoleName"]);
                        }
                    });


                bool status = (int)command.Parameters["@status"].Value == 0;
                return status ? rolenames.ToArray() : null;
            }
        }

        public bool IsUserInModule(string username, string moduleName, bool isLocal)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInModules_IsUserInModule]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@module_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    moduleName);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@is_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    isLocal);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);

                switch ((AlteaProviderStatusCode)command.Parameters["@status"].Value)
                {
                    case AlteaProviderStatusCode.False:
                        return false;

                    case AlteaProviderStatusCode.True:
                        return true;

                    case AlteaProviderStatusCode.InvalidUser:
                    case AlteaProviderStatusCode.InvalidData:
                    case AlteaProviderStatusCode.UnknownError:
                        return false;

                    default:
                        throw new ProviderException("Unknown error");
                }
            }
        }

        public bool IsUserInPermission(string username, string moduleName, string permissionName, bool isLocal)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInModules_IsUserInPermission]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@module_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    moduleName);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@permission_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    permissionName);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@is_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    isLocal);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.Altea);

                switch ((AlteaProviderStatusCode)command.Parameters["@status"].Value)
                {
                    case AlteaProviderStatusCode.False:
                        return false;

                    case AlteaProviderStatusCode.True:
                        return true;

                    case AlteaProviderStatusCode.InvalidUser:
                    case AlteaProviderStatusCode.InvalidData:
                    case AlteaProviderStatusCode.UnknownError:
                        return false;

                    default:
                        throw new ProviderException("Unknown error");
                }
            }
        }

        public IEnumerable<AlteaModule> GetModulesForUser(string username, bool isLocal)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            List<AlteaModule> modules = new List<AlteaModule>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInModules_GetModulesForUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@is_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    isLocal);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                AlteaModule module = new AlteaModule
                                    {
                                        Name = (string)reader["ModuleName"],
                                        Role = (string)reader["RoleName"],
                                        Priority = (int)reader["Priority"],
                                        Position = (int)reader["Position"]
                                    };

                                module.Route = AlteaModuleRouteDictionary.GetRoute(module.Name);
                                modules.Add(module);
                            }
                        });
            }

            return modules;
        }

        public AlteaModule GetHighestPriorityModuleForUser(string username, bool isLocal)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            AlteaModule module = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInModules_GetHighestPriorityModuleForUser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@username",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    username);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@is_local",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    isLocal);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        if (reader.Read())
                        {
                            module = new AlteaModule
                            {
                                Name = (string)reader["ModuleName"],
                                Role = (string)reader["RoleName"],
                                Priority = (int)reader["Priority"],
                                Position = (int)reader["Position"]
                            };

                            module.Route = AlteaModuleRouteDictionary.GetRoute(module.Name);
                        }
                    });
            }

            return module;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            List<string> usernames = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_UsersInRoles_GetUsersInRole]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@rolename",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    roleName);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            usernames = new List<string>();

                            while (reader.Read())
                            {
                                usernames.Add((string)reader["UserName"]);
                            }
                        });

                bool status = (int)command.Parameters["@status"].Value == 0;
                return status ? usernames.ToArray() : null;
            }
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllRoles(Guid appId)
        {
            List<string> roles = new List<string>();
            
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_GetAllRoles]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                roles.Add((string)reader["role"]);
                            }
                        });
            }

            return roles;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IDictionary<string, IEnumerable<string>>> GetAllModules(Guid appId)
        {
            IDictionary<string, IDictionary<string, IEnumerable<string>>> roles =
                new Dictionary<string, IDictionary<string, IEnumerable<string>>>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[altea_GetAllModules]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {
                        while (reader.Read())
                        {
                            string role = (string)reader["role"];
                            Dictionary<string, IEnumerable<string>> modules;

                            if (!roles.TryGetTypedValue(role, out modules))
                            {
                                modules = new Dictionary<string, IEnumerable<string>>();
                                roles.Add(role, modules);
                            }

                            string module = (string)reader["module"];
                            List<string> permissions;

                            if (!modules.TryGetTypedValue(module, out permissions))
                            {
                                permissions = new List<string>();
                                modules.Add(module, permissions);
                            }

                            string permission = reader["permission"] as string;
                            if (permission != null)
                            {
                                permissions.Add(permission);
                            }
                        }
                    });
            }

            return roles;
        } 
    }
}
