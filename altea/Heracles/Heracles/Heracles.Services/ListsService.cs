namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Altea.Classes.Lists;
    using Altea.Classes.Members;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;

    using CsQuery.ExtensionMethods;

    using Neo4jClient.Cypher;

    public class ListsService : Service<IListsChannel>
    {
        public const int MaxLists = 10;

        public static IEnumerable<ListCategory> GetCategories(Language language, ListType type)
        {
            int languageId = language.GetDatabaseId();

            Dictionary<int, ListCategory> categories =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match("(n:Category)-[r:EXISTS_IN]->(m:Language { id: { languageParams }.id })")
                    .WithParam("languageParams", new { id = languageId })
                    .Return(
                        (n, r) =>
                        new ListCategory
                            {
                                Id = Return.As<int>("n.id"),
                                Name = Return.As<string>("r.name"),
                                IsArea = Return.As<bool>("n.isArea"),
                                Position = Return.As<int>("r.position")
                            })
                    .Results
                    .ToDictionary(x => x.Id);
                    
            categories.ForEach(x => {x.Value.ListCount = new Dictionary<int, int>(); });

            Dictionary<int, ListCategory> rootCategories = new Dictionary<int, ListCategory>(categories);

            Comparer<ListCategory> comparer =
                Comparer<ListCategory>.Create(
                    (a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            // Parents and children
            ListCategory parent = null, child = null;
            GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                .Match(
                    "(n:Category)-[r:CONTAINS]->(m:Category)-[:EXISTS_IN]->(o:Language { id: { languageParams }.id })")
                .WithParam("languageParams", new { id = languageId })
                .Return(
                    (n, m) =>
                        new
                            {
                                Parent = Return.As<int>("n.id"),
                                Child = Return.As<int>("m.id")
                            })
                .OrderBy("n.id")
                .Results
                .ToList()
                .ForEach(
                    x =>
                        {
                            if ((parent == null || parent.Id != x.Parent) && !categories.TryGetValue(x.Parent, out parent))
                            {
                                return;
                            }

                            if ((child == null || child.Id != x.Child) && !categories.TryGetValue(x.Child, out child))
                                return;

                            if (parent.Children == null)
                            {
                                parent.Children = new SortedSet<ListCategory>(comparer);
                            }

                            ((SortedSet<ListCategory>)parent.Children).Add(child);
                            child.Parent = parent;
                            rootCategories.Remove(x.Child);
                        });
            
            // List count
            string queryString = type == ListType.All
                ? "(n:Category), (m:Language { id: { languageParams }.id }), (o:Type), (p:List), "
                : "(n:Category), (m:Language { id: { languageParams }.id }), (o:Type { id: { typeParams }.id }), (p:List), ";

            ICypherFluentQuery query =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match(
                        queryString
                        + "(p)-[:CATEGORIZED_AS]->(:Category)<-[:CONTAINS*0..10]-(n), "
                        + "(p)-[:EXISTS_IN]->(m), "
                        + "(p)-[:CLASSIFIED_AS]->(o)");

            if (type != ListType.All)
            {
                query = query.WithParam("typeParams", new { id = (int)type });
            }

            IEnumerable<ListCategoryCount> listsCount =
                query
                    .WithParam("languageParams", new { id = languageId })
                    .Return(
                        (n, m, o) =>
                            new ListCategoryCount
                                {
                                    Category = n.As<ListCategory>().Id,
                                    Type = (ListType)Return.As<int>("o.id"),
                                    Count = Return.As<int>("COUNT(DISTINCT p)")
                                })
                    .Results.ToArray();

            ListCategory lastCategory = null;
            foreach (ListCategoryCount listCount in listsCount)
            {
                if (lastCategory == null || lastCategory.Id != listCount.Category)
                {
                    categories.TryGetValue(listCount.Category, out lastCategory);
                }

                if (lastCategory != null)
                {
                    ((Dictionary<int, int>)lastCategory.ListCount).Add((int)listCount.Type, listCount.Count);
                }
            }

            foreach (ListCategory category in categories.Values.Where(category => category.ListCount == null))
            {
                if (category.Parent == null)
                {
                    rootCategories.Remove(category.Id);
                }
                else
                {
                    ((SortedSet<ListCategory>)category.Parent.Children).Remove(category);
                }
            }

            SortedSet<ListCategory> roots = new SortedSet<ListCategory>(rootCategories.Values, comparer);
            return roots;
        }

        public static IEnumerable<AlteaList> GetLists(Language language, ListType type)
        {
            int languageId = language.GetDatabaseId();

            string queryString = type == ListType.All
                ? "(n:List), (m:Language { id: { languageParams }.id }), (o:Type), (p:Data), "
                : "(n:List), (m:Language { id: { languageParams }.id }), (o:Type { id: { typeParams }.id }), (p:Data), ";

            ICypherFluentQuery query =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match(
                        queryString
                        + "(n)-[r:EXISTS_IN]->(m), "
                        + "(n)-[:CLASSIFIED_AS]->(o), "
                        + "(n)-[:CONTAINS]->(p)");

            if (type != ListType.All)
            {
                query = query.WithParam("typeParams", new { id = (int)type });
            }

            IDictionary<Guid, AlteaList> lists =
                query
                    .WithParam("languageParams", new { id = languageId })
                    .Return(
                        (n, r) =>
                            new AlteaList
                            {
                                Id = Return.As<Guid>("n.id"),
                                Type = Return.As<ListType>("o.id"),
                                Name = Return.As<string>("r.name"),
                                Description = Return.As<string>("r.description"),
                                Image = Return.As<string>("n.image"),
                                Count = Return.As<int>("COUNT(p)")
                            })
                    .Results
                    .ToDictionary(x => x.Id);

            queryString = type == ListType.All
                ? "(n:List), (m:Language { id: { languageParams }.id }), (p:Category), "
                : "(n:List), (m:Language { id: { languageParams }.id }), (o:Type { id: { typeParams }.id }), (p:Category), ";

           query =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match(
                        queryString
                        + "(n)-[:EXISTS_IN]->(m), "
                        + "(n)-[:CLASSIFIED_AS]->(o), "
                        + "(n)-[:CATEGORIZED_AS]->(p)");

            if (type != ListType.All)
            {
                query = query.WithParam("typeParams", new { id = (int)type });
            }

            AlteaList list = null;
            query
                .WithParam("languageParams", new { id = languageId })
                .Return(
                    (n, p) =>
                        new
                            {
                                List = Return.As<Guid>("n.id"),
                                Category = Return.As<int>("p.id")
                            })
                .OrderBy("n.id")
                .Results
                .ToList()
                .ForEach(
                    x =>
                    {
                        if ((list == null || list.Id != x.List) && !lists.TryGetValue(x.List, out list))
                        {
                            return;
                        }

                        if (list.Categories == null)
                        {
                            list.Categories = new List<int>();
                        }

                        ((List<int>)list.Categories).Add(x.Category);
                    });

            List<AlteaList> roots = lists.Values.ToList();
            roots.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return roots;
        }

        public static string GetCategoryName(int categoryId, Language language)
        {
            ICypherFluentQuery<string> query =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match(
                        "(n:Category { id: { categoryParams }.id })-[r:EXISTS_IN]->(m:Language { id: { languageParams }.id })")
                    .WithParam("categoryParams", new { id = categoryId })
                    .WithParam("languageParams", new { id = language.GetDatabaseId() })
                    .Return((n) => Return.As<string>("r.name"));

            return GraphDatabaseManager.GetResults(query).SingleOrDefault();
        }

        public static IEnumerable<ListGroupType> GetGroupTypes()
        {
            ICypherFluentQuery<ListGroupType> query =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match("(n:Group)")
                    .Return(n => n.As<ListGroupType>());

            return GraphDatabaseManager.GetResults(query);
        }

        public static IEnumerable<AssignedList> GetUserLists(Guid userId, ListType type, Language language)
        {
            ICypherFluentQuery<AssignedList> query =
                GraphDatabaseManager.Cypher(GraphConnectionString.ListRepository)
                    .Match(
                        "(n:List)",
                        "(n)-[r:ASSIGNED_TO]->(m:User)",
                        "(n)-[:CLASSIFIED_AS]->(o:Type)",
                        "(n)-[s:EXISTS_IN]->(p:Language)",
                        "(n)-[:SPLIT]->(q:Group)",
                        "(n)-[:CONTAINS]->(t:Data)-[:CLASSIFIED_AS]->(o)")
                    .Where((User m) => m.Id == userId)
                    .AndWhere("o.id = { typeId }")
                    .AndWhere("p.id = { languageId }")
                    .WithParam("typeId", (int)type)
                    .WithParam("languageId", language.GetDatabaseId())
                    .Return(
                        (n, r, s) =>
                        new AssignedList
                            {
                                Id = Return.As<Guid>("n.id"),
                                Name = Return.As<string>("s.name"),
                                Position = Return.As<int>("r.position"),
                                InboxFactor = Return.As<int>("r.inboxFactor"),
                                DataCount = Return.As<int>("COUNT(t)"),
                                GroupType = Return.As<int>("q.id")
                            });

            return GraphDatabaseManager.GetResults(query);
        }

        public static IDictionary<Guid, AssignedListStatus> GetUserAssignments(Guid userId, ListType type, Language language, IEnumerable<Guid> lists)
        {
            Dictionary<Guid, AssignedListStatus> status = new Dictionary<Guid, AssignedListStatus>(lists.Count());

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[LISTS_GetUserAssignments]"))
            using (DataTable listsTable = new DataTable())
            {
                listsTable.Columns.Add("n", typeof(Guid));

                foreach (Guid list in lists)
                {
                    DataRow row = listsTable.NewRow();
                    row["n"] = list;
                    listsTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@user",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@type",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    (int)type);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@lists",
                    ParameterDirection.Input,
                    "[dbo].[guidlist]",
                    listsTable);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            while (reader.Read())
                            {
                                Guid list = (Guid)reader["list"];

                                AssignedListStatus listStatus = new AssignedListStatus
                                    {
                                        SectionId = (int)reader["section"],
                                        CategoryId = (int)reader["category"],
                                        Assigned = (int)reader["assigned"],
                                        Inboxed = (int)reader["inboxed"],
                                        Accepted = (int)reader["accepted"],
                                        Rejected = (int)reader["rejected"],
                                        WorkedAndRejected = (int)reader["worked_and_rejected"],
                                        Finished = (int)reader["finished"],
                                        Recognized = (int)reader["recognized"]
                                    };

                                status.Add(list, listStatus);
                            }
                        });
            }

            return status;
        }
    }
}
