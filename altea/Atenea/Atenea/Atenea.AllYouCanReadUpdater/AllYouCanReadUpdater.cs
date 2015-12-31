namespace Atenea.AllYouCanReadUpdater
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    using Altea.Common.Classes;
    using Altea.Extensions;

    using CsQuery;
    
    using Microsoft.WindowsAzure.Storage.Blob;

    public partial class AllYouCanReadUpdater
    {
        private readonly CloudBlobContainer blobContainer;
        private readonly MethodInfo runMethod;

        public delegate void AllYouCanReadEventHandler(string message);
        public event AllYouCanReadEventHandler Message;

        private static readonly int LanguageId = Language.English.GetDatabaseId();

        public AllYouCanReadUpdater(CloudBlobContainer container, AllYouCanReadType type)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.blobContainer = container;
            this.runMethod = this.GetType().GetMethod(type + "Run", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Run()
        {
            this.runMethod.Invoke(this, null);
        }

        private delegate TResult Func<in T1, T2, out TResult>(T1 obj1, out T2 obj2);

        private static bool LoadUrl<T>(Uri url, Func<Uri, T, bool> loadFunction, out T result)
        {
            result = default(T);

            bool status = false;
            int count = 10;

            while (!status && count > 0)
            {
                try
                {
                    status = loadFunction.Invoke(url, out result);
                }
                catch
                {
                    status = false;
                }

                count--;
            }

            return status;
        }

        private static bool LoadUrl(Uri url, out CQ doc)
        {
            return AllYouCanReadUpdater.LoadUrl<CQ>(
                url,
                (Uri u, out CQ d) =>
                    {
                        d = CQ.CreateFromUrl(url.AbsoluteUri);
                        return true;
                    },
                out doc);
        }

        private static bool LoadUrl(Uri url, out Uri responseUrl)
        {
            return AllYouCanReadUpdater.LoadUrl<Uri>(
                url,
                (Uri u, out Uri ru) =>
                    {
                        HttpWebResponse response = null;
                        try
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            response = (HttpWebResponse)request.GetResponse();
                            ru = response.ResponseUri;
                            return true;
                        }
                        finally
                        {
                            if (response != null)
                            {
                                response.Dispose();
                            }
                        }
                    },
                out responseUrl);
        }

        private static bool LoadUrl(Uri url, out Stream stream)
        {
            return AllYouCanReadUpdater.LoadUrl<Stream>(
                url,
                (Uri u, out Stream s) =>
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(u);

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            if (responseStream == null)
                            {
                                s = null;
                                return false;
                            }

                            s = new MemoryStream();
                            byte[] buffer = new byte[16 * 1024];
                            int read;

                            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                s.Write(buffer, 0, read);
                            }
                        }

                        s.Position = 0;
                        return true;
                    },
                out stream);
        }

        private static string GetContentType(string extension)
        {
            switch (extension)
            {
                case "bmp":
                    return "image/bmp";

                case "jpe":
                case "jpeg":
                case "jpg":
                    return "image/jpeg";

                case "gif":
                    return "image/gif";

                case "png":
                    return "image/png";

                default:
                    return "application/octet-stream";
            }
        }

        private static string GetMd5Sum(Stream stream)
        {
            string sum;
            using (HashAlgorithm hash = MD5.Create())
            {
                sum = hash.ComputeHash(stream)
                    .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2")))
                    .ToString();
            }

            stream.Position = 0;
            return sum;
        }

    }
}
