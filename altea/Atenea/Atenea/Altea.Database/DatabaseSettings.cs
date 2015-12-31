namespace Altea.Database
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;

    public static class DatabaseSettings
    {
        private static ConnectionStringsOrigin connectionStringsWarehouse = ConnectionStringsOrigin.ConfigurationManager;

        private static ApplicationSettingsBase settingsFile;
        private static Dictionary<string, string> customDictionary; 

        public static ConnectionStringsOrigin ConnectionStringsWarehouse
        {
            get
            {
                return connectionStringsWarehouse;
            }

            set
            {
                if (value == connectionStringsWarehouse)
                {
                    return;
                }

                settingsFile = null;
                customDictionary = null;

                connectionStringsWarehouse = value;
            }
        }

        public static ApplicationSettingsBase SettingsFile
        {
            get
            {
                if (connectionStringsWarehouse != ConnectionStringsOrigin.SettingsFile)
                {
                    throw new InvalidOperationException("Database Warehouse not set with a settings file.");
                }

                return settingsFile;
            }

            set
            {
                if (connectionStringsWarehouse != ConnectionStringsOrigin.SettingsFile)
                {
                    throw new InvalidOperationException("Database Warehouse not set with a settings file.");
                }

                settingsFile = value;
            }
        }

        public static Dictionary<string, string> CustomDictionary
        {
            get
            {
                if (connectionStringsWarehouse != ConnectionStringsOrigin.CustomDictionary)
                {
                    throw new InvalidOperationException("Database Warehouse not set with a custom dictionary.");
                }

                return customDictionary;
            }

            set
            {
                if (connectionStringsWarehouse != ConnectionStringsOrigin.CustomDictionary)
                {
                    throw new InvalidOperationException("Database Warehouse not set with a custom dictionary.");
                }

                customDictionary = value;
            }
        }

        private const int DefaultConnectionRetries = 5;

        private static bool ConnectionRetriesSetted = false;

        private static int connectionRetries;
        public static int ConnectionRetries
        {
            get
            {
                return connectionRetries;
            }
            set
            {
                ConnectionRetriesSetted = true;
                connectionRetries = value;
            }
        }

        private const int DefaultConnectionRetryWaitSeconds = 2;

        private static bool ConnectionRetryWaitSecondsSetted = false;

        private static int connectionRetryWaitSeconds;
        public static int ConnectionRetryWaitSeconds
        {
            get
            {
                return connectionRetryWaitSeconds;
            }
            set
            {
                ConnectionRetryWaitSecondsSetted = true;
                connectionRetryWaitSeconds = value;
            }
        }

        internal static string GetConnectionString(string connectionString)
        {
            switch (ConnectionStringsWarehouse)
            {
                case ConnectionStringsOrigin.ConfigurationManager:
                    if (!ConnectionRetriesSetted)
                    {
                        try
                        {
                            ConnectionRetries = int.Parse(
                                ConfigurationManager.AppSettings["ConnectionRetries"],
                                CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            ConnectionRetries = DefaultConnectionRetries;
                        }
                    }

                    if (!ConnectionRetryWaitSecondsSetted)
                    {
                        try
                        {
                            ConnectionRetryWaitSeconds =
                                int.Parse(
                                    ConfigurationManager.AppSettings["ConnectionRetryWaitSeconds"],
                                    CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            ConnectionRetryWaitSeconds = DefaultConnectionRetryWaitSeconds;
                        }
                    }

                    return ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;

                case ConnectionStringsOrigin.SettingsFile:
                    if (!ConnectionRetriesSetted)
                    {
                        try
                        {
                            ConnectionRetries = (int)settingsFile["ConnectionRetries"];
                        }
                        catch
                        {
                            ConnectionRetries = DefaultConnectionRetries;
                        }
                    }

                    if (!ConnectionRetryWaitSecondsSetted)
                    {
                        try
                        {
                            ConnectionRetryWaitSeconds = (int)settingsFile["ConnectionRetryWaitSeconds"];
                        }
                        catch
                        {
                            ConnectionRetryWaitSeconds = DefaultConnectionRetryWaitSeconds;
                        }
                    }

                    return settingsFile[connectionString] as string;

                case ConnectionStringsOrigin.CustomDictionary:
                    if (!ConnectionRetriesSetted)
                    {
                        ConnectionRetries = DefaultConnectionRetries;
                    }
                    
                    if (!ConnectionRetryWaitSecondsSetted)
                    {
                        ConnectionRetryWaitSeconds = DefaultConnectionRetryWaitSeconds;
                    }

                    return customDictionary[connectionString];

                default:
                    throw new InvalidOperationException("No Warehouse specified in Database Settings.");
            }
        }
    }
}
