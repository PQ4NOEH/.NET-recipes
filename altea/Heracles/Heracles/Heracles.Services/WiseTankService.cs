namespace Heracles.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using System.Data;
    using System.Data.SqlClient;

    using Altea.Classes.WiseTank;
    using Altea.Contracts;
    using Altea.Database;

    public abstract partial class WiseTankService : Service<IWiseTankChannel>
    {
        public static IEnumerable<TankRole> GetRoles()
        {
            Dictionary<int, TankRole> roles = new Dictionary<int, TankRole>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_GetRoles]"))
            {
                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["id"];
                            TankRole role = new TankRole
                            {
                                Id = id,
                                Name = (string)reader["lowered_name"],
                                Position = (int)reader["position"],
                                Selectable = (bool)reader["selectable"],
                                Permissions = new List<TankRolePermission>()
                            };

                            roles.Add(id, role);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            int roleId = (int)reader["role"];
                            TankRole role;
                            roles.TryGetValue(roleId, out role);

                            TankRolePermission permission = new TankRolePermission
                            {
                                Id = (int)reader["permission_id"],
                                Name = (string)reader["permission_name"],
                                Position = (int)reader["permission_position"],
                                Permissions = new TankPermissions
                                {
                                    ListUsers = (bool)reader["list_users"],
                                    InviteUsers = (bool)reader["invite_users"],
                                    ReadArticles = (bool)reader["read_articles"],
                                    WriteArticles = (bool)reader["write_articles"],
                                    UnmoderatedArticles = (bool)reader["unmoderated_articles"],
                                    VoteArticles = (bool)reader["vote_articles"],
                                    AssignArticles = (bool)reader["assign_articles"],
                                    ApproveArticles = (bool)reader["approve_articles"],
                                    DeleteArticles = (bool)reader["delete_articles"],
                                    ReadComments = (bool)reader["read_comments"],
                                    WriteComments = (bool)reader["write_comments"],
                                    WritePrivateComments = (bool)reader["write_private_comments"],
                                    UnmoderatedComments = (bool)reader["unmoderated_comments"],
                                    ApproveComments = (bool)reader["approve_comments"],
                                    DeleteComments = (bool)reader["delete_comments"],
                                    HideAuthor = (bool)reader["hide_author"],
                                    CreateSubTimelines = (bool)reader["create_subtimelines"],
                                    OverrideOwnArticles = reader["override_own_articles"] as bool?
                                }
                            };

                            ((List<TankRolePermission>)role.Permissions).Add(permission);
                        }
                    });
            }

            return roles.Values.ToList();
        }

    }
}
