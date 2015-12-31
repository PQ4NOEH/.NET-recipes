// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionString.cs" company="ALTEA">
//   No copyright yet.
// </copyright>
// <summary>
//   Type safe enumeration pattern for SQL Server connection strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Altea.Database
{
    /// <summary>
    /// Type safe enumeration pattern for SQL Server connection strings.
    /// </summary>
    public static class SqlConnectionString
    {
        /// <summary>
        /// Common application data and settings.
        /// </summary>
        public const string Altea = "Altea";

        /// <summary>
        /// Common user data and settings.
        /// </summary>
        public const string DataWarehouse = "DataWarehouse";

        /// <summary>
        /// Vocabulary dictionary database.
        /// </summary>
        public const string Dictionary = "Dictionary";

        /// <summary>
        /// Term Defis encyclopaedia database.
        /// </summary>
        public const string Encyclopaedia = "Encyclopaedia";
    }
}
