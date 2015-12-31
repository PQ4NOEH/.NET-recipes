namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Stats;
    using Altea.Common.Classes;
    using Altea.Database;
    using Altea.Extensions;

    /// <summary>
    /// The stats service.
    /// </summary>
    public abstract class StatsService
    {
        private static readonly Dictionary<string, IEnumerable<string>> _moduleSettings =
            new Dictionary<string, IEnumerable<string>>
                {
                    { "wordstax", new string[] { "vocabulary_inbox_overflow", "vocabulary_stax_underflow", "vocabulary_stax_overflow" } },
                    { "termstax", new string[] { } },
                    { "gramstax", new string[] { } }
                };

        public static IReadOnlyDictionary<string, IEnumerable<string>> ModuleSettings
        {
            get
            {
                return _moduleSettings;
            }
        }

        public static IEnumerable<ModuleStats> GetStats(
            Guid userId,
            Guid appId,
            Language languageFrom,
            Language languageTo,
            int? level,
            int? proLevel,
            int? proLevelSub,
            IEnumerable<string> modules)
        {
            if (modules == null) return new List<ModuleStats>();
            List<ModuleStats> stats = new List<ModuleStats>(modules.Count());

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[STATS_GetStats]"))

            using (DataTable modulesTable = new DataTable())
            {
                modulesTable.Columns.Add("n", typeof(string));

                foreach (string module in modules)
                {
                    DataRow row = modulesTable.NewRow();
                    row["n"] = module;
                    modulesTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    appId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    languageFrom.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    languageTo.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@level",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    level);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@prolevel",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    proLevel);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@prolevel_sub",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    proLevelSub);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@modules",
                    ParameterDirection.Input,
                    "[dbo].[stringlist]",
                    modulesTable);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            do
                            {
                                List<ModuleStatsData> moduleStatsData = null;

                                while (reader.Read())
                                {
                                    if (moduleStatsData == null)
                                    {
                                        moduleStatsData = new List<ModuleStatsData>();

                                        ModuleStats module = new ModuleStats
                                            {
                                                Name = (string)reader["module"],
                                                Stats = moduleStatsData
                                            };

                                        stats.Add(module);
                                    }

                                    moduleStatsData.Add(new ModuleStatsData
                                        {
                                            Name = (string)reader["name"],
                                            Value = (string)reader["value"],
                                            Status = (ModuleStatsStatus)(int)reader["status"]
                                        });
                                }
                            }
                            while (reader.NextResult());
                        });

                return stats;
            }
        }

        public static void SetStatus(ModuleStats module, IDictionary<string, string> settingsData)
        {
            switch (module.Name)
            {
                case "wordstax":
                    SetWordStaxStatus(module, settingsData);
                    break;
            }
        }

        private static void SetWordStaxStatus(ModuleStats module, IDictionary<string, string> settingsData)
        {
            ModuleStatsData inboxStats = module.Stats.SingleOrDefault(x => x.Name == "inbox");
            if (inboxStats != null)
            {
                int inboxOverflow = Convert.ToInt32(settingsData["vocabulary_inbox_overflow"]),
                    inboxWarning = (int)(inboxOverflow * 0.75),
                    inboxDanger = (int)(inboxOverflow * 0.90),
                    inboxValue = Convert.ToInt32(inboxStats.Value);

                if (inboxValue >= inboxDanger)
                {
                    inboxStats.Status = ModuleStatsStatus.Danger;
                }
                else if (inboxValue >= inboxWarning)
                {
                    inboxStats.Status = ModuleStatsStatus.Warning;
                }
                else
                {
                    inboxStats.Status = ModuleStatsStatus.Good;
                }
            }

            ModuleStatsData staxStats = module.Stats.SingleOrDefault(x => x.Name == "stax");
            if (staxStats != null)
            {
                int staxUnderflow = Convert.ToInt32(settingsData["vocabulary_stax_underflow"]),
                    staxOverflow = Convert.ToInt32(settingsData["vocabulary_stax_overflow"]),
                    staxWarning = (staxOverflow - (staxUnderflow / 2)) * WordStaxService.MaxStack,
                    staxDanger = (staxOverflow - (staxUnderflow / 4)) * WordStaxService.MaxStack,
                    staxValue = Convert.ToInt32(staxStats.Value);

                if (staxValue >= staxDanger)
                {
                    staxStats.Status = ModuleStatsStatus.Danger;
                }
                else if (staxValue >= staxWarning)
                {
                    staxStats.Status = ModuleStatsStatus.Warning;
                }
                else
                {
                    staxStats.Status = ModuleStatsStatus.Good;
                }

            }
        }
    }
}
