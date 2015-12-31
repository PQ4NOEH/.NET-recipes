// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphDatabaseManager.cs" company="ALTEA">
//   No copyright yet.
// </copyright>
// <summary>
//   Static class for querying Neo4j databases.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Altea.Database
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    using Neo4jClient;
    using Neo4jClient.Cypher;

    /// <summary>
    /// Static class for querying Neo4j databases. DON'T use Neo4jClient library
    /// from NuGet. Instead, use https://github.com/xabinapal/Neo4jClient.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Spell check on URI.")]
    public static class GraphDatabaseManager
    {
        /// <summary>
        /// Dictionary with created database clients.
        /// </summary>
        private static readonly ConcurrentDictionary<string, GraphClient> Instances = new ConcurrentDictionary<string, GraphClient>();

        private static readonly ConcurrentDictionary<string, X509Certificate2> Certificates = new ConcurrentDictionary<string, X509Certificate2>(); 

        /// <summary>
        /// Lock object for creating new clients. Dictionary is thread-safe, lock is used to avoid creating
        /// two GraphClient objects and connecting them to the same database prior to dictionary insertion.
        /// </summary>
        private static readonly object InstanceLock = new object();

        /// <summary>
        /// Lock object for storing certificates.
        /// </summary>
        private static readonly object CertificatesLock = new object();

        /// <summary>
        /// Maximum number of retries before aborting queries.
        /// </summary>
        private static readonly int ConnectionRetries;

        /// <summary>
        /// Number of seconds to wait before first retry of a query.
        /// </summary>
        private static readonly int ConnectionRetryWaitSeconds;

        static GraphDatabaseManager()
        {
            ConnectionRetries = DatabaseSettings.ConnectionRetries;
            ConnectionRetryWaitSeconds = DatabaseSettings.ConnectionRetryWaitSeconds;
        }

        /// <summary>
        /// Stores the certificate used when connecting to a database.
        /// </summary>
        /// <param name="connectionString">the connection string of the database</param>
        /// <param name="certificate">the certificate used in the connection</param>
        /// <param name="protocol">the SSL protocol used</param>
        /// <returns></returns>
        public static bool StoreCertificate(string connectionString, X509Certificate2 certificate, SecurityProtocolType protocol)
        {
            if (Certificates.ContainsKey(connectionString))
            {
                return false;
            }

            lock (CertificatesLock)
            {
                if (!Certificates.ContainsKey(connectionString))
                {
                    ServicePointManager.SecurityProtocol |= protocol;
                    Certificates.TryAdd(connectionString, certificate);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the Cypher object of a database, used to make queries against it.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="ICypherFluentQuery"/>.
        /// </returns>
        public static ICypherFluentQuery Cypher(string connectionString)
        {
            X509Certificate2 certificate;
            Certificates.TryGetValue(connectionString, out certificate);
            GraphClient client = GetConnection(connectionString, certificate);
            return client.Cypher;
        }

        /// <summary>
        /// Gets the Cypher object of a database, used to make queries against it.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="certificate">
        /// The certificate.
        /// </param>
        /// <param name="protocol">
        /// The security protocol used.
        /// </param>
        /// <returns>
        /// The <see cref="ICypherFluentQuery"/>.
        /// </returns>
        public static ICypherFluentQuery Cypher(string connectionString, X509Certificate2 certificate, SecurityProtocolType protocol)
        {
            ServicePointManager.SecurityProtocol |= protocol;
            GraphClient client = GetConnection(connectionString, certificate);
            return client.Cypher;
        }

        /// <summary>
        /// Calculates the number of seconds to wait between query retries.
        /// </summary>
        /// <param name="attempt">
        /// The attempt number.
        /// </param>
        /// <returns>
        /// The number of seconds to wait.
        /// </returns>
        private static int GetConnectionRetryWaitSeconds(int attempt)
        {
            // Backoff Throttling
            // http://blogs.msdn.com/b/sqlazure/archive/2010/05/11/10011247.aspx
            return ConnectionRetryWaitSeconds * (int)Math.Pow(2, attempt);
        }

        /// <summary>
        /// Retrieves an existing Neo4j connection or creates a new one.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="certificate">
        /// The certificate.
        /// </param>
        /// <returns>
        /// The <see cref="GraphClient"/>.
        /// </returns>
        private static GraphClient GetConnection(string connectionString, X509Certificate2 certificate)
        {
            GraphClient connection;
            Instances.TryGetValue(connectionString, out connection);

            if (connection != null)
            {
                return connection;
            }

            lock (InstanceLock)
            {
                Instances.TryGetValue(connectionString, out connection);

                if (connection != null)
                {
                    return connection;
                }

                Uri uri = new Uri(DatabaseSettings.GetConnectionString(connectionString));
                HttpClientWrapper wrapper;

                if (uri.Scheme.ToUpperInvariant() == "HTTPS" && certificate != null)
                {
                    WebRequestHandler handler = new WebRequestHandler
                        {
                            ServerCertificateValidationCallback =
                                (sender, x509Certificate, chain, errors) => certificate.Equals(x509Certificate)
                        };

                    HttpClient client = new HttpClient(handler);
                    wrapper = new HttpClientWrapper(client);
                }
                else
                {
                    wrapper = new HttpClientWrapper();
                }

                connection = new GraphClient(uri, wrapper);

                for (int attempt = 1;;)
                {
                    try
                    {
                        connection.Connect();
                        break;
                    }
                    catch
                    {
                        if (++attempt == ConnectionRetries)
                        {
                            throw;
                        }

                        Thread.Sleep(GetConnectionRetryWaitSeconds(attempt));
                    }
                }

                Instances.TryAdd(connectionString, connection);
            }

            return connection;
        }

        /// <summary>
        /// Executes a neo4j query and return its results.
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="query">cypher query to execute</param>
        /// <returns>the result of the query</returns>
        public static IEnumerable<T> GetResults<T>(ICypherFluentQuery<T> query)
        {
            IEnumerable<T> results;

            for (int attempt = 1;;)
            {
                try
                {
                    results = query.Results;
                    break;
                }
                catch
                {
                    if (++attempt == ConnectionRetries)
                    {
                        throw;
                    }

                    Thread.Sleep(GetConnectionRetryWaitSeconds(attempt));
                }
            }

            return results;
        }
    }
}
