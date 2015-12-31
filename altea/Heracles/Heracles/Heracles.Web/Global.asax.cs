namespace Heracles.Web
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Altea.Database;
    using Altea.Extensions;

    using Heracles.Web.Resources;

    using Microsoft.Azure;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DatabaseSettings.ConnectionStringsWarehouse = ConnectionStringsOrigin.ConfigurationManager;

            X509Certificate2 certificate = new X509Certificate2(ListsResources.certificate);
            GraphDatabaseManager.StoreCertificate(GraphConnectionString.ListRepository, certificate, SecurityProtocolType.Tls12);

            AppCore.GetAppName = this.GetApplicationName;
            AppCore.GetAppId = this.GetApplicationId;
            AppCore.GetAppSettings = this.GetApplicationSettings;
            AppCore.GetInstanceSettings = this.GetInstanceSettings;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(float), new FloatingPointNumberModelBinder());
            ModelBinders.Binders.Add(typeof(double), new FloatingPointNumberModelBinder());
            ModelBinders.Binders.Add(typeof(decimal), new FloatingPointNumberModelBinder());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            AlteaCache.Init();
        }

        protected void Application_EndRequest()
        {
            // Don't redirect unlogged AJAX Requests, send Unauthorised instead
            if (this.Context.Response.StatusCode == 302
                && this.Context.Request.Headers["X-Requested-With"] == "XMLHttpWebRequest")
            {
                this.Context.Response.Clear();
                this.Context.Response.StatusCode = 401;
            }
        }

        private string GetApplicationName()
        {
            string applicationName = CloudConfigurationManager.GetSetting("Altea.ApplicationName");
            string name = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.Text,
                    "SELECT [name] FROM [dbo].[altea_Applications] WHERE [lowered_name] = @application_name;"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    applicationName);

                name = SqlDatabaseManager.ExecuteScalar<string>(command, SqlConnectionString.Altea);
            }

            return name;
        }

        private Guid GetApplicationId()
        {
            string applicationName = CloudConfigurationManager.GetSetting("Altea.ApplicationName");
            Guid? applicationId;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.Text,
                    "SELECT [id] FROM [dbo].[altea_Applications] WHERE [lowered_name] = @application_name;"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application_name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    applicationName);

                applicationId = SqlDatabaseManager.ExecuteScalar<Guid?>(command, SqlConnectionString.Altea);
            }

            return applicationId ?? Guid.Empty;
        }

        private Dictionary<string, string> GetApplicationSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.Text,
                    "SELECT * FROM [dbo].[altea_WebGeneralSettings]"))
            {
                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                    {

                        while (reader.Read())
                        {
                            settings.Add((string)reader["option"], (string)reader["value"]);
                        }
                    });
            }

            return settings;
        }

        private bool GetInstanceSettings()
        {
            using (SqlCommand command = SqlDatabaseManager.CreateCommand(
                CommandType.Text,
                "SELECT * FROM [dbo].[altea_ApplicationSettings] WHERE [id] = @application_id;"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@application_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    AppCore.AppId);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Altea,
                    reader =>
                        {
                            reader.Read();
                            this.ParseInstanceSettings(reader);
                        });
            }

            return true;
        }

        private void ParseInstanceSettings(SqlDataReader reader)
        {
            const string prefixName = "Setting_";
            const int prefixLength = 8;
            const string arraySeparator = ";";
            const string castMethodName = "Cast";

            IEnumerable<PropertyInfo> settings =
                typeof(AppCore).GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.Name.StartsWith(prefixName));

            foreach (PropertyInfo setting in settings)
            {
                string name = setting.Name.Substring(prefixLength).CamelCaseToSnakeCase();

                object settingData;
                try
                {
                    settingData = reader[name];
                }
                catch
                {
                    // Property doesn't exists in the database
                    continue;
                }

                Type settingType = settingData.GetType();

                if (setting.PropertyType == settingType)
                {
                    setting.SetValue(null, Convert.ChangeType(settingData, settingType));
                }
                else
                {
                    string settingDataString = (string)settingData;

                    if (setting.PropertyType.IsArray)
                    {
                        Type settingArrayType = setting.PropertyType.GetElementType();
                        string[] settingDataStringSplit = settingDataString.Split(
                            new string[] { arraySeparator },
                            StringSplitOptions.RemoveEmptyEntries);

                        Array array = Array.CreateInstance(settingArrayType, settingDataStringSplit.Length);
                        setting.SetValue(null, array);

                        MethodInfo parseMethod = settingArrayType.GetMethod(
                            "Parse",
                            BindingFlags.Public | BindingFlags.Static);

                        MethodInfo castMethod =
                            typeof(DynamicExtensions).GetMethod(castMethodName, BindingFlags.Public | BindingFlags.Static)
                                .MakeGenericMethod(settingArrayType);

                        if (settingDataStringSplit.Length != 0)
                        {
                            // ReSharper disable once PossibleNullReferenceException
                            for (int i = 0; i < settingDataStringSplit.Length; i++)
                            {
                                try
                                {
                                    object settingDataParsed = (parseMethod ?? castMethod).Invoke(
                                        null,
                                        new object[] { settingDataStringSplit[i] });

                                    array.SetValue(settingDataParsed, i);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                    else
                    {
                        MethodInfo parseMethod = setting.PropertyType.GetMethod(
                            "Parse",
                            BindingFlags.Public | BindingFlags.Static);

                        MethodInfo castMethod =
                            typeof(DynamicExtensions).GetMethod(castMethodName, BindingFlags.Public | BindingFlags.Static)
                                .MakeGenericMethod(setting.PropertyType);
                        try
                        {
                            object settingDataParsed = (parseMethod ?? castMethod).Invoke(
                                null,
                                new object[] { settingDataString });

                            setting.SetValue(null, settingDataParsed);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
        }
    }
}
