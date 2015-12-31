namespace Altea.Services
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;

    using Altea.Classes.WiseTank;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.WiseTank;

    /// <summary>
    /// The wise tank service.
    /// </summary>
    public partial class WiseTankService : IService, IWiseTankContract
    {
        public WiseTankError CreateArticle(WiseTankCreateArticleModel model)
        {
            WiseTankError error;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_CreateArticle]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.UserId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.AppId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@timeline_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.Timeline);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@origin",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    (int)model.Origin);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@reference",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    string.IsNullOrWhiteSpace(model.Reference) ? null : model.Reference);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@source",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    string.IsNullOrWhiteSpace(model.Source) ? null : model.Source);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@favicon",
                    ParameterDirection.Input,
                    SqlDbType.VarBinary,
                    WiseTankService.GetIcon(model.Favicon));

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    string.IsNullOrWhiteSpace(model.Name) ? null : model.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@lead",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    model.Lead);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@description",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    string.IsNullOrWhiteSpace(model.Description) ? null : model.Description);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@image",
                    ParameterDirection.Input,
                    SqlDbType.VarBinary,
                    WiseTankService.GetImage(model.Image));

                SqlDatabaseManager.AddParameter(
                    command,
                    "@offset_date",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.OffsetDate);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                error = (WiseTankError)((int)command.Parameters["@status"].Value);
            }

            return error;
        }

        private static byte[] GetIcon(string url)
        {
            byte[] iconArray = null;

            Uri iconUri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out iconUri))
            {
                return null;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(iconUri);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return null;
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        switch (response.ContentType.ToLowerInvariant())
                        {
                            case "image/vnd.microsoft.icon":
                            case "image/x-icon":
                            case "image/ico":
                            case "image/icon":
                            case "text/ico":
                            case "application/ico":
                                using (Bitmap bitmap = new Bitmap(responseStream))
                                {
                                    bitmap.Save(stream, ImageFormat.Png);
                                }

                                iconArray = stream.ToArray();
                                break;

                            default:
                                using (Image image = Image.FromStream(responseStream, false, true))
                                {
                                    image.Save(stream, ImageFormat.Png);
                                }

                                iconArray = stream.ToArray();
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return iconArray;
        }

        private static byte[] GetImage(string url)
        {
            byte[] imageArray = null;

            Uri imageUri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out imageUri))
            {
                return null;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageUri);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return null;
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (Image image = Image.FromStream(responseStream, false, true))
                        {
                            image.Save(stream, ImageFormat.Jpeg);
                        }

                        imageArray = stream.ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }


            return imageArray;
        }

        public int? Vote(WiseTankVoteModel model)
        {
            int? votes;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_VoteArticle]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@article",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Article);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@upvote",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    model.UpVote);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@votes",
                    ParameterDirection.Output,
                    SqlDbType.Int);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                votes = command.Parameters["@votes"].Value as int?;
            }

            return votes;
        }

        public decimal? Karma(WiseTankKarmaModel model)
        {
            decimal? karma;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISETANK_KarmaArticle]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@user_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.User);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@app_id",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    model.App);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@article",
                    ParameterDirection.Input,
                    SqlDbType.BigInt,
                    model.Article);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@userKarma",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    model.Karma);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@karma",
                    ParameterDirection.Output,
                    SqlDbType.Decimal);

                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                karma = command.Parameters["@karma"].Value as decimal?;
            }

            return karma;
        }
    }
}
