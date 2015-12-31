namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.WiseNet;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models;

    public abstract partial class WiseNetService : Service<IWiseNetChannel>
    {
        public static IEnumerable<WiseNetSearchEngine> GetSearchEngines(Language language)
        {
            IEnumerable<WiseNetSearchEngine> searchEngines = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetSearchEngines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => { searchEngines = WiseNetService.GetSearchEngines(reader); });
            }

            return searchEngines;
        }

        public static IEnumerable<WiseNetCarousel> GetMagazines(Language language)
        {
            IEnumerable<WiseNetCarousel> carousels = null;
 
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetMagazines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => { carousels = GetMagazines(reader); });
            }

            return carousels;
        }

        static void CheckUserSearchEngines(Guid userId, Language language)
        {
            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_CheckUserSearchEngines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
            }
        }

        public static IEnumerable<WiseNetSearchEngine> GetUserSearchEngines(Guid userId, Language language)
        {
            CheckUserSearchEngines(userId, language);

            IEnumerable<WiseNetSearchEngine> searchEngines = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetUserSearchEngines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => { searchEngines = WiseNetService.GetSearchEngines(reader); });
            }

            return searchEngines;
        }

        public static IEnumerable<WiseNetCarousel> GetUserMagazines(Guid userId, Language language)
        {
            IEnumerable<WiseNetCarousel> carousels = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetUserMagazines]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader => { carousels = GetMagazines(reader); });
            }

            return carousels;
        }

        private static IEnumerable<WiseNetSearchEngine> GetSearchEngines(SqlDataReader reader)
        {
            Dictionary<int, WiseNetSearchEngine> searchEngines = new Dictionary<int, WiseNetSearchEngine>();

            while (reader.Read())
            {
                int id = (int)reader["id"];

                searchEngines.Add(
                    id,
                    new WiseNetSearchEngine
                        {
                            Id = id,
                            Name = (string)reader["name"],
                            Description = reader["description"] as string,
                            Url = (string)reader["url"],
                            SearchUrl = reader["search_url"] as string,
                            Image = reader["image"] as string,
                            Position = (int)reader["position"],
                            Visible = (bool)reader["visible"],
                            Default = (bool)reader["default"],
                            Sections = new List<WiseNetSearchEngineSection>()
                        });
            }

            reader.NextResult();

            while (reader.Read())
            {
                int engineId = (int)reader["search_engine"];
                List<WiseNetSearchEngineSection> engine =
                    (List<WiseNetSearchEngineSection>)searchEngines[engineId].Sections;

                engine.Add(
                    new WiseNetSearchEngineSection
                        {
                            Id = (int)reader["id"],
                            Name = (string)reader["name"],
                            Description = reader["description"] as string,
                            Url = (string)reader["url"],
                            Image = reader["image"] as string,
                            Position = (int)reader["position"],
                            Visible = (bool)reader["visible"]
                        });
            }

            return searchEngines.Select(x => x.Value);
        }

        private static IEnumerable<WiseNetCarousel> GetMagazines(SqlDataReader reader)
        {
            Dictionary<int, WiseNetCarousel> carousels = new Dictionary<int, WiseNetCarousel>();

            while (reader.Read())
            {
                int id = (int)reader["id"];
                WiseNetCarousel carousel = new WiseNetCarousel
                                               {
                                                   Id = id,
                                                   Name = (string)reader["name"],
                                                   Position = (int)reader["position"],
                                                   Visible = (bool)reader["visible"],
                                                   Magazines = new List<WiseNetMagazine>()
                                               };

                carousels.Add(id, carousel);
            }

            reader.NextResult();

            while (reader.Read())
            {
                int carouselId = (int)reader["carousel_id"];
                WiseNetCarousel carousel;
                if (!carousels.TryGetValue(carouselId, out carousel))
                {
                    continue;
                }

                WiseNetMagazine magazine = new WiseNetMagazine
                    {
                        Id = (int)reader["id"],
                        Custom = (bool)reader["custom"],
                        Name = (string)reader["name"],
                        Description = reader["description"] as string,
                        Url = (string)reader["url"],
                        Image = reader["image"] as string,
                        Screenshot = reader["screenshot"] as string,
                        Favicon = reader["favicon"] as string,
                        Position = (int)reader["position"],
                        Visible = (bool)reader["visible"]
                    };

                ((List<WiseNetMagazine>)carousel.Magazines).Add(magazine);
            }

            return carousels.Values;
        }
    }
}
