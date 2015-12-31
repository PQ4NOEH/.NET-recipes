using System;
using System.Configuration;

namespace Heracles.Web
{
    public class AppConfiguration
    {
        public static Lazy<AppConfiguration> Configuration = new Lazy<AppConfiguration>(()=> new AppConfiguration());

        public string AlteaDataConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DataWarehouse"].ConnectionString;
            }
        }

        public string AlteaConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Altea"].ConnectionString;
            }
        }

        public string Encyclopaedia
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Dictionary"].ConnectionString;
            }
        }
    }
}