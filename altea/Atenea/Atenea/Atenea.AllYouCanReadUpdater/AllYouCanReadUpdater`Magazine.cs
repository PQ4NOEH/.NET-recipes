namespace Atenea.AllYouCanReadUpdater
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    using Altea.Database;

    using CsQuery;
    using CsQuery.Implementation;

    using Microsoft.WindowsAzure.Storage.Blob;

    public partial class AllYouCanReadUpdater
    {
        private static readonly Uri MagazinesUri = new Uri("http://www.allyoucanread.com");

        private void MagazineRun()
        {
            CQ indexDocument;
            if (!AllYouCanReadUpdater.LoadUrl(MagazinesUri, out indexDocument))
            {
                return;
            }

            HashSet<Magazine> allMagazines = new HashSet<Magazine>();
            HashSet<Carousel> allCarousels = new HashSet<Carousel>();

            IEnumerable<Uri> carouselUris = AllYouCanReadUpdater.MagazineGetCarousels(indexDocument);
            List<Carousel> carousels = new List<Carousel>();
            
            foreach (Uri carouselUri in carouselUris)
            {
                try
                {
                    Carousel carousel = AllYouCanReadUpdater.MagazineGetCarousel(
                        carouselUri,
                        allMagazines,
                        allCarousels);

                    if (carousel != null)
                    {
                        carousels.Add(carousel);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            this.MagazineSaveCarousels(carousels);

            foreach (Carousel carousel in allCarousels)
            {
                foreach (Magazine magazine in carousel.Magazines)
                {
                    try
                    {
                        bool status = this.MagazineSave(magazine, carousel.Id);

                        if (!status && this.Message != null)
                        {
                            this.Message(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not save magazine {0} in carousel {1} ({2})",
                                    magazine.Name,
                                    carousel.Id,
                                    carousel.Name));
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        private static IEnumerable<Uri> MagazineGetCarousels(CQ document)
        {
            CQ carousels = document.Select("div.country-newspapers-side div.magazinecategories a.categorytitle2");
            return (
                from HtmlAnchorElement carousel in carousels
                select new Uri(carousel.Href)).ToArray();
        }

        private static Carousel MagazineGetCarousel(Uri uri, HashSet<Magazine> allMagazines, HashSet<Carousel> allCarousels)
        {
            CQ carouselDocument;
            if (!AllYouCanReadUpdater.LoadUrl(uri, out carouselDocument))
            {
                return null;
            }

            string title = AllYouCanReadUpdater.MagazineGetCarouselTitle(carouselDocument);
            IList<Magazine> magazines = AllYouCanReadUpdater.MagazineGetMagazines(uri, carouselDocument, allMagazines);

            if (!magazines.Any())
            {
                return null;
            }

            Carousel carousel = new Carousel(title, magazines);

            IEnumerable<Carousel> childrenCarousels =
                AllYouCanReadUpdater.MagazineFindChildrenCarousels(carouselDocument, title, allMagazines, allCarousels);
            foreach (Carousel child in childrenCarousels)
            {
                carousel.AddChild(child);
            }

            allCarousels.Add(carousel);

            return carousel;
        }

        private static string MagazineGetCarouselTitle(CQ carouselDocument)
        {
            DomElement nameNode = carouselDocument.Select("div.wrapper div.titlewrapper h1")[0] as DomElement;
            string name = nameNode.InnerText;
            name = name.Substring(0, name.Length - 11).Trim();

            if (name.StartsWith("Top 10", StringComparison.InvariantCultureIgnoreCase))
            {
                name = name.Substring(6).Trim();
            }

            return HttpUtility.HtmlDecode(name);
        }

        private static IList<Magazine> MagazineGetMagazines(Uri thisUri, CQ carouselDocument, HashSet<Magazine> allMagazines)
        {
            HashSet<Magazine> magazines = new HashSet<Magazine>();
            CQ magazineNodes = carouselDocument.Select("div.top10mag, div.mag");

            foreach (DomElement magazine in magazineNodes)
            {
                try
                {
                    CQ magazineNode = magazine.Cq();
                    CQ titleNodes = magazineNode.Find("div.magtitle a");

                    if (titleNodes == null || titleNodes.Length <= 0)
                    {
                        continue;
                    }

                    HtmlAnchorElement titleNode = magazineNode.Find("div.magtitle a")[0] as HtmlAnchorElement;

                    if (titleNode == null)
                    {
                        continue;
                    }

                    string name = HttpUtility.HtmlDecode(titleNode.InnerText.Trim());

                    Uri partialHref = new Uri(titleNode.Href, UriKind.RelativeOrAbsolute);
                    Uri href = new Uri(thisUri, partialHref);

                    Uri finalHref;
                    if (!AllYouCanReadUpdater.LoadUrl(href, out finalHref))
                    {
                        break;
                    }

                    Magazine mag =
                        allMagazines.SingleOrDefault(
                            x => x.Name.Equals(name.ToLowerInvariant()) || x.Url.Equals(finalHref));

                    if (mag == null)
                    {
                        CQ subscribeNodes = magazineNode.Find("div.subscribe_large a");
                        if (subscribeNodes == null || subscribeNodes.Length == 0)
                        {
                            subscribeNodes = magazineNode.Find("div.subscribe a");
                        }

                        HtmlAnchorElement subscribeNode = subscribeNodes[1] as HtmlAnchorElement;
                        Uri subscribePartialHref = new Uri(subscribeNode.Href, UriKind.RelativeOrAbsolute);
                        Uri subscribeHref = new Uri(thisUri, subscribePartialHref);
                        DomElement coverImageNode =
                            magazineNode.Find("div.mag_image img")[0] as DomElement;
                        Uri cover = new Uri(coverImageNode.Attributes["src"], UriKind.Absolute);

                        string description;
                        CQ subscribeDoc;
                        if (AllYouCanReadUpdater.LoadUrl(subscribeHref, out subscribeDoc))
                        {
                            CQ descriptionNodes = subscribeDoc.Select("span.product-description-short-content p");
                                
                            if (descriptionNodes == null || descriptionNodes.Length == 0)
                            {
                                descriptionNodes = subscribeDoc.Select("span.product-description-short-content");
                            }
                                
                            if (descriptionNodes != null && descriptionNodes.Length != 0)
                            {
                                CQ descriptionNode = descriptionNodes.Clone();
                                descriptionNode.Children().Remove();
                                description = HttpUtility.HtmlDecode(descriptionNode.Text().Trim());
                                if (string.IsNullOrEmpty(description))
                                {
                                    description = null;
                                }
                            }
                            else
                            {
                                description = null;
                            }
                        }
                        else
                        {
                            description = null;
                        }

                        mag = new Magazine
                                  {
                                      Name = name,
                                      Description = description,
                                      Url = finalHref,
                                      Cover = cover
                                  };
                        allMagazines.Add(mag);
                    }

                    magazines.Add(mag);
                }
                catch
                {
                    // ignored
                }
            }

            return magazines.ToList();
        }

        private static IEnumerable<Carousel> MagazineFindChildrenCarousels(
            CQ carouselDocument,
            string parentName,
            HashSet<Magazine> allMagazines,
            HashSet<Carousel> allCarousels)
        {
            List<Carousel> carousels = new List<Carousel>();

            string subtitleText = parentName + " Magazine Categories";
            CQ subtitles = carouselDocument.Find("div.wrapper h2");

            foreach (DomElement subtitle in subtitles)
            {
                if (
                    !subtitleText.Equals(
                        subtitle.InnerText,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                DomElement subcategory = subtitle.NextElementSibling as DomElement;

                if (subcategory == null)
                {
                    continue;
                }

                CQ subCarousels = subcategory.Cq().Find("a.categorytitle2");

                foreach (HtmlAnchorElement anchor in subCarousels)
                {
                    if (anchor.Href == null)
                    {
                        continue;
                    }

                    Uri href = new Uri(anchor.Href);
                    Carousel carousel = AllYouCanReadUpdater.MagazineGetCarousel(href, allMagazines, allCarousels);

                    if (carousel != null)
                    {
                        carousels.Add(carousel);
                    }
                }

                break;
            }

            return carousels;
        }

        private bool MagazineSave(Magazine magazine, int carouselId)
        {
            string imageUri = this.MagazineSaveCover(magazine.Name, magazine.Cover);

            bool status;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_SaveMagazine]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    LanguageId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    magazine.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@description",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    magazine.Description);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@url",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    magazine.Url.AbsoluteUri);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@carousel",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    carouselId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@is_user_carousel",
                    ParameterDirection.Input,
                    SqlDbType.Bit,
                    false);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@image",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    imageUri);

                SqlDatabaseManager.AddParameter(command, "@status", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);

                status = (int)command.Parameters["@status"].Value == 0;
            }

            return status;
        }

        private string MagazineSaveCover(string magazineName, Uri magazineCover)
        {
            string title;
            using (HashAlgorithm hash = MD5.Create())
            {
                title =
                    hash.ComputeHash(Encoding.ASCII.GetBytes(magazineName))
                        .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2")))
                        .ToString();
            }

            string fileType = magazineCover.AbsolutePath;
            fileType = fileType.Substring(fileType.LastIndexOf('.') + 1).ToLowerInvariant();
            string contentType = AllYouCanReadUpdater.GetContentType(fileType);

            CloudBlockBlob blob = this.blobContainer.GetBlockBlobReference(title + "." + fileType);

            Stream stream;
            if (!AllYouCanReadUpdater.LoadUrl(magazineCover, out stream))
            {
                return null;
            }

            try
            {
                string sum = AllYouCanReadUpdater.GetMd5Sum(stream);

                bool upload;
                if (blob.Exists())
                {
                    blob.FetchAttributes();
                    upload = sum.Equals(blob.Properties.ContentMD5);
                }
                else
                {
                    upload = true;
                }

                if (upload)
                {
                    blob.Properties.ContentType = contentType;
                    blob.Properties.ContentMD5 = sum;
                    blob.UploadFromStream(stream);
                    blob.SetProperties();
                    blob.CreateSnapshot();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return title;
        }

        private void MagazineSaveCarousels(IEnumerable<Carousel> carousels, int? parent = null)
        {
            foreach (Carousel carousel in carousels)
            {
                try
                {
                    int id = AllYouCanReadUpdater.MagazineSaveCarousel(carousel, parent);
                    carousel.Id = id;

                    if (id > 0 && carousel.Children.Any())
                    {
                        this.MagazineSaveCarousels(carousel.Children, id);
                    }
                    else if (id == 0 && this.Message != null)
                    {
                        this.Message(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not save carousel {0}",
                                carousel.Name));
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static int MagazineSaveCarousel(Carousel carousel, int? parent)
        {
            int id;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_SaveCarousel]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    LanguageId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@name",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    carousel.Name);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@parent",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    parent);

                SqlDatabaseManager.AddParameter(command, "@id", ParameterDirection.ReturnValue, SqlDbType.Int);
                SqlDatabaseManager.ExecuteNonQuery(command, SqlConnectionString.DataWarehouse);
                id = (int?)command.Parameters["@id"].Value ?? 0;
            }

            return id;
        }
    }
}
