// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlteaCache.cs" company="">
//   
// </copyright>
// <summary>
//   The altea cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Heracles.Web
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web;
    using System.Web.Caching;

    using Altea.Extensions;

    using Microsoft.ApplicationServer.Caching;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using StackExchange.Redis;

    /// <summary>
    /// The altea cache.
    /// </summary>
    public static class AlteaCache
    {
        /// <summary>
        /// The scope.
        /// </summary>
        public enum Scope
        {
            /// <summary>
            /// ASP.NET internal cache, local to each role instance.
            /// </summary>
            Instance, 

            /// <summary>
            /// Role cache, shared between instance roles.
            /// </summary>
            Role, 

            /// <summary>
            /// Redis cache shared between all Heracles instances.
            /// </summary>
            Altea
        }
        
        /// <summary>
        /// The term.
        /// </summary>
        public enum Term
        {
            /// <summary>
            /// Absolute expiration of objects (10 minutes).
            /// </summary>
            [TermData(Minutes = 10, IsSliding = false)]
            Short,

            /// <summary>
            /// Sliding expiration of objects (30 minutes).
            /// </summary>
            [TermData(Minutes = 30, IsSliding = true)]
            Medium,

            /// <summary>
            /// Absolute expiration of objects (1 day).
            /// </summary>
            [TermData(Minutes = 1440, IsSliding = false)]
            Large,

            /// <summary>
            /// Absolute expiration of objects (1 week).
            /// </summary>
            [TermData(Minutes = 10080, IsSliding = false)]
            Largest
        }

        /// <summary>
        /// The term data attribute.
        /// </summary>
        private class TermDataAttribute : Attribute
        {
            /// <summary>
            /// Gets or sets the minutes.
            /// </summary>
            public int Minutes { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is sliding.
            /// </summary>
            public bool IsSliding { get; set; }
        }

        /// <summary>
        /// The asp net cache.
        /// </summary>
        private static class AspNetCache
        {
            /// <summary>
            /// The cache.
            /// </summary>
            private static readonly Cache Cache = HttpContext.Current.Cache;

            /// <summary>
            /// The lock.
            /// </summary>
            private static readonly object Lock = new object();

            /// <summary>
            /// The keys.
            /// </summary>
            private static readonly HashSet<string> Keys = new HashSet<string>();

            /// <summary>
            /// The get.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool Get<T>(string key, out T value)
            {
                object data = Cache[key];

                if (data == null)
                {
                    value = default(T);
                    return false;
                }

                value = (T) data;
                return true;
            }

            /// <summary>
            /// The get or insert.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool GetOrInsert<T>(string key, Func<T> action, Term term, out T value)
            {
                object data = Cache[key];

                if (data == null)
                {
                    lock (Lock)
                    {
                        data = Cache[key];

                        if (data == null)
                        {
                            object result = action.Invoke();
                            value = (T)result;

                            if (result == null)
                            {
                                return false;
                            }

                            TermDataAttribute attribute = term.GetAttribute<TermDataAttribute>();
                            DateTime absoluteExpiration;
                            TimeSpan slidingExpiration;

                            if (attribute.IsSliding)
                            {
                                absoluteExpiration = Cache.NoAbsoluteExpiration;
                                slidingExpiration = TimeSpan.FromMinutes(attribute.Minutes);
                            }
                            else
                            {
                                absoluteExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(attribute.Minutes));
                                slidingExpiration = Cache.NoSlidingExpiration;
                            }

                            Cache.Insert(key, result, null, absoluteExpiration, slidingExpiration);
                            Keys.Add(key);
                            return true;
                        }

                        value = (T)data;
                        return false;
                    }
                }

                value = (T) data;
                return true;
            }

            /// <summary>
            /// The insert or update.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool InsertOrUpdate<T>(string key, Func<T> action, Term term)
            {
                object result = action.Invoke();

                if (result == null)
                {
                    return false;
                }

                TermDataAttribute attribute = term.GetAttribute<TermDataAttribute>();
                DateTime absoluteExpiration;
                TimeSpan slidingExpiration;

                if (attribute.IsSliding)
                {
                    absoluteExpiration = Cache.NoAbsoluteExpiration;
                    slidingExpiration = TimeSpan.FromMinutes(attribute.Minutes);
                }
                else
                {
                    absoluteExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(attribute.Minutes));
                    slidingExpiration = Cache.NoSlidingExpiration;
                }

                lock (Lock)
                {
                    Cache.Insert(key, result, null, absoluteExpiration, slidingExpiration);
                    Keys.Add(key);
                    return true;
                }
            }

            /// <summary>
            /// The insert or update.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool InsertOrUpdate<T>(string key, Func<T> action, Term term, out T value)
            {
                object result = action.Invoke();
                value = (T)result;

                if (result == null)
                {
                    return false;
                }

                TermDataAttribute attribute = term.GetAttribute<TermDataAttribute>();
                DateTime absoluteExpiration;
                TimeSpan slidingExpiration;

                if (attribute.IsSliding)
                {
                    absoluteExpiration = Cache.NoAbsoluteExpiration;
                    slidingExpiration = TimeSpan.FromMinutes(attribute.Minutes);
                }
                else
                {
                    absoluteExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(attribute.Minutes));
                    slidingExpiration = Cache.NoSlidingExpiration;
                }

                lock (Lock)
                {
                    Cache.Insert(key, result, null, absoluteExpiration, slidingExpiration);
                    Keys.Add(key);
                    return true;
                }
            }

            internal static bool UpdateIfExists<T>(string key, Func<T> action, Term term, out T value)
            {
                value = default(T);
                return false;
            }

            /// <summary>
            /// The remove key.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            internal static void RemoveKey(string key)
            {
                lock (Lock)
                {
                    Cache.Remove(key);
                    Keys.Remove(key);
                }
            }

            /// <summary>
            /// The remove all keys.
            /// </summary>
            internal static void RemoveAllKeys()
            {
                lock (Lock)
                {
                    foreach (string key in Keys)
                    {
                        Cache.Remove(key);
                    }

                    Keys.Clear();
                }
            }
        }

        /// <summary>
        /// The in role cache.
        /// </summary>
        private static class InRoleCache
        {
            /// <summary>
            /// The caches.
            /// </summary>
            private static readonly Dictionary<Term, DataCache> Caches;

            /// <summary>
            /// The lock.
            /// </summary>
            private static readonly object Lock = new object();

            /// <summary>
            /// The max retry count.
            /// </summary>
            private static readonly int MaxRetryCount = int.Parse(ConfigurationManager.AppSettings["ConnectionRetries"]);

            /// <summary>
            /// The base retry wait seconds.
            /// </summary>
            private static readonly int BaseRetryWaitSeconds = int.Parse(ConfigurationManager.AppSettings["ConnectionRetryWaitSeconds"]);

            static InRoleCache()
            {
                DataCacheFactory cacheFactory = new DataCacheFactory();

                Caches = new Dictionary<Term, DataCache>()
                    {
                        { Term.Short, cacheFactory.GetCache("default") },
                        { Term.Medium, cacheFactory.GetCache("medium") },
                        { Term.Large, cacheFactory.GetCache("large") },
                        { Term.Largest, cacheFactory.GetCache("largest") }
                    };
            }

            /// <summary>
            /// The connection retry wait seconds.
            /// </summary>
            /// <param name="attempt">
            /// The attempt.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            private static int ConnectionRetryWaitSeconds(int attempt)
            {
                // Backoff Throttling
                return BaseRetryWaitSeconds * (int)Math.Pow(2, attempt);
            }

            /// <summary>
            /// The retry litmus.
            /// </summary>
            /// <param name="dataCacheException">
            /// The data cache exception.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            private static bool RetryLitmus(DataCacheException dataCacheException)
            {
                // Codes from Enterprise Library 6 TransientFaultHandling Module
                switch (dataCacheException.ErrorCode)
                {
                    case 16:
                    case 17:
                    case 18:
                    case 17016:
                        return true;

                    default:
                        return false;
                }
            }

            /// <summary>
            /// The get.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool Get<T>(string key, Term term, out T value)
            {
                object result;
                for (int attempt = 1; ; )
                {
                    try
                    {
                        result = Caches[term].Get(key);
                        break;
                    }
                    catch (DataCacheException dataCacheException)
                    {
                        if (++attempt == MaxRetryCount || !RetryLitmus(dataCacheException))
                        {
                            throw;
                        }

                        Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                    }
                    catch (TimeoutException)
                    {
                        if (++attempt == MaxRetryCount)
                        {
                            throw;
                        }

                        Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                    }
                }

                if (result == null)
                {
                    value = default(T);
                    return false;
                }

                value = (T) result;
                return true;
            }

            /// <summary>
            /// The get or insert.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool GetOrInsert<T>(string key, Func<T> action, Term term, out T value)
            {
                DataCache cache = Caches[term];

                object result;
                bool getStatus = Get(key, term, out result);

                if (getStatus)
                {
                    value = (T)result;
                    return false;
                }

                lock (Lock)
                {
                    getStatus = Get(key, term, out result);

                    if (!getStatus)
                    {
                        result = action.Invoke();
                        value = (T)result;

                        if (result == null)
                        {
                            return false;
                        }

                        for (int attempt = 1; ; )
                        {
                            try
                            {
                                cache.Put(key, result);
                                break;
                            }
                            catch (DataCacheException dataCacheException)
                            {
                                if (++attempt == MaxRetryCount || !RetryLitmus(dataCacheException))
                                {
                                    throw;
                                }

                                Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                            }
                            catch (TimeoutException)
                            {
                                if (++attempt == MaxRetryCount)
                                {
                                    throw;
                                }

                                Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                            }
                        }

                        return true;
                    }

                    value = (T)result;
                    return false;
                }
            }

            /// <summary>
            /// The insert or update.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool InsertOrUpdate<T>(string key, Func<T> action, Term term)
            {
                DataCache cache = Caches[term];

                object result = action.Invoke();

                if (result == null)
                {
                    return false;
                }

                lock (Lock)
                {
                    for (int attempt = 1; ; )
                    {
                        try
                        {
                            cache.Put(key, result);
                            break;
                        }
                        catch (DataCacheException dataCacheException)
                        {
                            if (++attempt == MaxRetryCount || !RetryLitmus(dataCacheException))
                            {
                                throw;
                            }

                            Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                        }
                        catch (TimeoutException)
                        {
                            if (++attempt == MaxRetryCount)
                            {
                                throw;
                            }

                            Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                        }
                    }

                    return true;
                }
            }

            /// <summary>
            /// The insert or update.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool InsertOrUpdate<T>(string key, Func<T> action, Term term, out T value)
            {
                DataCache cache = Caches[term];

                object result = action.Invoke();
                value = (T)result;

                if (result == null)
                {
                    return false;
                }

                lock (Lock)
                {
                    for (int attempt = 1; ; )
                    {
                        try
                        {
                            cache.Put(key, result);
                            break;
                        }
                        catch (DataCacheException dataCacheException)
                        {
                            if (++attempt == MaxRetryCount || !RetryLitmus(dataCacheException))
                            {
                                throw;
                            }

                            Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                        }
                        catch (TimeoutException)
                        {
                            if (++attempt == MaxRetryCount)
                            {
                                throw;
                            }

                            Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                        }
                    }

                    return true;
                }
            }

            internal static bool UpdateIfExists<T>(string key, Func<T> action, Term term, out T value)
            {
                value = default(T);
                return false;
            }

            /// <summary>
            /// The remove key.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            internal static void RemoveKey(string key, Term term)
            {
                lock (Lock)
                {
                    for (int attempt = 1;;)
                    {
                        try
                        {
                            Caches[term].Remove(key);
                            break;
                        }
                        catch (DataCacheException dataCacheException)
                        {
                            if (++attempt == MaxRetryCount || !RetryLitmus(dataCacheException))
                            {
                                throw;
                            }

                            Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                        }
                        catch (TimeoutException)
                        {
                            if (++attempt == MaxRetryCount)
                            {
                                throw;
                            }

                            Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                        }
                    }
                }
            }

            /// <summary>
            /// The remove all keys.
            /// </summary>
            internal static void RemoveAllKeys()
            {
                lock (Lock)
                {
                    foreach (DataCache cache in Caches.Values)
                    {
                        for (int attempt = 1;;)
                        {
                            try
                            {
                                cache.Clear();
                                break;
                            }
                            catch (DataCacheException dataCacheException)
                            {
                                if (++attempt == MaxRetryCount || !RetryLitmus(dataCacheException))
                                {
                                    throw;
                                }

                                Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                            }
                            catch (TimeoutException)
                            {
                                if (++attempt == MaxRetryCount)
                                {
                                    throw;
                                }

                                Thread.Sleep(ConnectionRetryWaitSeconds(attempt));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The redis cache.
        /// </summary>
        private static class RedisCache
        {
            /// <summary>
            /// The _redis connection.
            /// </summary>
            private static ConnectionMultiplexer _redisConnection;

            /// <summary>
            /// The _redis cache database.
            /// </summary>
            private static IDatabase _redisCacheDatabase;

            /// <summary>
            /// The separator.
            /// </summary>
            private const char Separator = (char)0x1d; /* GS, group separator */

            /// <summary>
            /// The create key.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            /// <exception cref="CacheException">
            /// </exception>
            private static string CreateKey(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new CacheException(Scope.Altea, "Empty cache key name", new ArgumentNullException("name"));
                }

                return RoleEnvironment.DeploymentId + Separator + name;
            }

            /// <summary>
            /// The retrieve key name.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            /// <exception cref="CacheException">
            /// </exception>
            private static string RetrieveKeyName(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new CacheException(Scope.Altea, "Empty cache key", new ArgumentNullException("key"));
                }

                string[] groups = key.Split(Separator);

                if (groups.Length < 2)
                {
                    throw new CacheException(Scope.Altea, @"Invalid cache key: " + key, new ArgumentOutOfRangeException("key"));
                }

                // Really? Key name has control characters? This shouldn't happen, but anyway...
                return groups.Length == 2 ? groups[1] : string.Join("\u001d", groups.Skip(1));
            }

            /// <summary>
            /// The check status.
            /// </summary>
            internal static void CheckStatus()
            {
                if (_redisConnection != null && _redisConnection.IsConnected)
                {
                    return;
                }

                try
                {
                    _redisConnection =
                        ConnectionMultiplexer.Connect(
                            ConfigurationManager.ConnectionStrings["RedisCache"].ConnectionString);
                    _redisCacheDatabase = _redisConnection.GetDatabase();
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                    Debug.WriteLine(e.Message);
#else
                    new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
                    throw;
                }
            }

            /// <summary>
            /// The get.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="global">
            /// The global.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool Get<T>(string key, bool global, Term term, out T value)
            {
                CheckStatus();

                RedisValue result = _redisCacheDatabase.StringGet(global ? key : RetrieveKeyName(key), CommandFlags.PreferSlave);

                if (result.IsNull)
                {
                    value = default(T);
                    return false;
                }

                value = ((string)result).FromJson<T>();
                return true;
            }

            /// <summary>
            /// The get or insert.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="global">
            /// The global.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool GetOrInsert<T>(string key, bool global, Func<T> action, Term term, out T value)
            {
                CheckStatus();
                
                RedisValue result = _redisCacheDatabase.StringGet(global ? key : RetrieveKeyName(key), CommandFlags.PreferSlave);
                
                if (result.IsNull)
                {
                    TermDataAttribute attribute = term.GetAttribute<TermDataAttribute>();
                    TimeSpan expiration = TimeSpan.FromMinutes(attribute.Minutes);

                    value = action.Invoke();
                    _redisCacheDatabase.StringSet(
                        global ? key : RetrieveKeyName(key), 
                        value.ToJson(), 
                        expiration, 
                        When.NotExists, 
                        CommandFlags.FireAndForget);
                    return true;
                }
                else
                {
                    value = ((string)result).FromJson<T>();
                    return false;
                }
            }

            /// <summary>
            /// The insert or update.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool InsertOrUpdate<T>(string key, Func<T> action, Term term)
            {
                // TODO
                return false;
            }

            /// <summary>
            /// The insert or update.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <typeparam name="T">
            /// </typeparam>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool InsertOrUpdate<T>(string key, Func<T> action, Term term, out T value)
            {
                //TODO
                value = default(T);
                return false;
            }

            internal static bool UpdateIfExists<T>(string key, Func<T> action, Term term, out T value)
            {
                value = default(T);
                return false;
            }

            /// <summary>
            /// The remove key.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <param name="term">
            /// The term.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            internal static bool RemoveKey(string key, Term term)
            {
                return false;
            }

            /// <summary>
            /// The remove all keys.
            /// </summary>
            internal static void RemoveAllKeys()
            {
                CheckStatus();

                EndPoint[] endpoints = _redisConnection.GetEndPoints(true);

                foreach (EndPoint endpoint in endpoints)
                {
                    IServer server = _redisConnection.GetServer(endpoint);
                    server.FlushAllDatabases();
                }
            }
        }

        /// <summary>
        /// The cache exception.
        /// </summary>
        [Serializable]
        public class CacheException : Exception
        {
            /// <summary>
            /// The _cache scope.
            /// </summary>
            private readonly Scope _cacheScope;

            /// <summary>
            /// Gets the cache scope.
            /// </summary>
            public Scope CacheScope
            {
                get { return this._cacheScope; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheException"/> class.
            /// </summary>
            /// <param name="scope">
            /// The scope.
            /// </param>
            public CacheException(Scope scope)
            {
                this._cacheScope = scope;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheException"/> class.
            /// </summary>
            /// <param name="scope">
            /// The scope.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            public CacheException(Scope scope, string message)
                : base(message)
            {
                this._cacheScope = scope;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheException"/> class.
            /// </summary>
            /// <param name="scope">
            /// The scope.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="innerException">
            /// The inner exception.
            /// </param>
            public CacheException(Scope scope, string message, Exception innerException)
                : base(message, innerException)
            {
                this._cacheScope = scope;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheException"/> class.
            /// </summary>
            /// <param name="info">
            /// The info.
            /// </param>
            /// <param name="context">
            /// The context.
            /// </param>
            [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
            protected CacheException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                this._cacheScope = (Scope)info.GetValue("CacheException.CacheScope", typeof(Scope));
            }

            /// <summary>
            /// The get object data.
            /// </summary>
            /// <param name="info">
            /// The info.
            /// </param>
            /// <param name="context">
            /// The context.
            /// </param>
            [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("CacheException.CacheScope", this._cacheScope);
            }
        }

        /// <summary>
        /// The _init.
        /// </summary>
        private static bool init;

        private static Dictionary<string, ManualResetEvent> keyLocks;

        private static object keyLockObj;

        /// <summary>
        /// The init.
        /// </summary>
        public static void Init()
        {
            AlteaCache.init = true;

            AlteaCache.keyLocks = new Dictionary<string, ManualResetEvent>();
            AlteaCache.keyLockObj = new object();

            RedisCache.CheckStatus();
        }

        /// <summary>
        /// Gets a value from cache.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="global">
        /// The global.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// true if value found, false if not.
        /// </returns>
        public static bool Get<T>(string key, bool? global, Scope scope, Term term, out T value)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }
            
            if (!Enum.IsDefined(typeof(Term), term))
            {
                throw new ArgumentOutOfRangeException("term");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        return AspNetCache.Get(key, out value);

                    case Scope.Role:
                        return InRoleCache.Get(key, term, out value);

                    case Scope.Altea:
                        return RedisCache.Get(key, global ?? true, term, out value);

                    default:
                        value = default(T);
                        return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Gets a value from cache. If key doesn't exists, inserts it.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="global">
        /// The global.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// true if value was inserted, false if not.
        /// </returns>
        public static bool GetOrInsert<T>(string key, bool? global, Func<T> action, Scope scope, Term term, out T value)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }

            if (!Enum.IsDefined(typeof(Term), term))
            {
                throw new ArgumentOutOfRangeException("term");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        return AspNetCache.GetOrInsert(key, action, term, out value);

                    case Scope.Role:
                        return InRoleCache.GetOrInsert(key, action, term, out value);

                    case Scope.Altea:
                        return RedisCache.GetOrInsert(key, global ?? true, action, term, out value);

                    default:
                        value = action.Invoke();
                        return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
                value = action.Invoke();
                return false;
            }
        }

        /// <summary>
        /// Inserts a value in cache. If key already exists, updates it.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <returns>
        /// true if value was inserted, false if already existed.
        /// </returns>
        public static bool InsertOrUpdate<T>(string key, Func<T> action, Scope scope, Term term)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }

            if (!Enum.IsDefined(typeof(Term), term))
            {
                throw new ArgumentOutOfRangeException("term");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        return AspNetCache.InsertOrUpdate(key, action, term);

                    case Scope.Role:
                        return InRoleCache.InsertOrUpdate(key, action, term);

                    case Scope.Altea:
                        return RedisCache.InsertOrUpdate(key, action, term);

                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
                return false;
            }
        }

        /// <summary>
        /// Inserts a value in cache. If key already exists, updates it.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// true if value was inserted, false if already existed.
        /// </returns>
        public static bool InsertOrUpdate<T>(string key, Func<T> action, Scope scope, Term term, out T value)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }

            if (!Enum.IsDefined(typeof(Term), term))
            {
                throw new ArgumentOutOfRangeException("term");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        return AspNetCache.InsertOrUpdate(key, action, term, out value);

                    case Scope.Role:
                        return InRoleCache.InsertOrUpdate(key, action, term, out value);

                    case Scope.Altea:
                        return RedisCache.InsertOrUpdate(key, action, term, out value);

                    default:
                        value = action.Invoke();
                        return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
                value = action.Invoke();
                return false;
            }
        }

        /// <summary>
        /// Updates a value un cache, only if key is already inserted.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// true if value was inserted or updated, false if not.
        /// </returns>
        public static bool UpdateIfExists<T>(string key, Func<T> action, Scope scope, Term term, out T value)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }

            if (!Enum.IsDefined(typeof(Term), term))
            {
                throw new ArgumentOutOfRangeException("term");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        return AspNetCache.UpdateIfExists(key, action, term, out value);

                    case Scope.Role:
                        return InRoleCache.UpdateIfExists(key, action, term, out value);

                    case Scope.Altea:
                        return RedisCache.UpdateIfExists(key, action, term, out value);

                    default:
                        value = action.Invoke();
                        return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
                value = action.Invoke();
                return false;
            }
        }

        /// <summary>
        /// The remove key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public static void RemoveKey(string key, Scope scope, Term term)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }

            if (!Enum.IsDefined(typeof(Term), term))
            {
                throw new ArgumentOutOfRangeException("term");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        AspNetCache.RemoveKey(key);
                        break;

                    case Scope.Role:
                        InRoleCache.RemoveKey(key, term);
                        break;

                    case Scope.Altea:
                        RedisCache.RemoveKey(key, term);
                        break;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
            }
        }

        /// <summary>
        /// The remove all keys.
        /// </summary>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public static void RemoveAllKeys(Scope scope)
        {
            if (!init)
            {
                throw new InvalidOperationException("Cache not initialized");
            }

            if (!Enum.IsDefined(typeof(Scope), scope))
            {
                throw new ArgumentOutOfRangeException("scope");
            }

            try
            {
                switch (scope)
                {
                    case Scope.Instance:
                        AspNetCache.RemoveAllKeys();
                        break;

                    case Scope.Role:
                        InRoleCache.RemoveAllKeys();
                        break;

                    case Scope.Altea:
                        RedisCache.RemoveAllKeys();
                        break;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("EXCEPTION: Overriding Raygun exception logging.");
                Debug.WriteLine(e.Message);
#else
                new Mindscape.Raygun4Net.RaygunClient().SendInBackground(e);
#endif
            }
        }

        #region Keys
        public static void LockKey(string key)
        {
            ManualResetEvent mres;
            if (!AlteaCache.keyLocks.TryGetValue(key, out mres))
            {
                lock (keyLockObj)
                {
                    if (!AlteaCache.keyLocks.TryGetValue(key, out mres))
                    {
                        mres = new ManualResetEvent(false);
                        AlteaCache.keyLocks.Add(key, mres);
                        return;
                    }
                }
            }
            
            mres.WaitOne();
            lock (keyLockObj)
            {
                mres.Reset();
            }
        }

        public static void UnlockKey(string key)
        {
            ManualResetEvent mres;
            if (!AlteaCache.keyLocks.TryGetValue(key, out mres))
            {
                lock (keyLockObj)
                {
                    if (AlteaCache.keyLocks.TryGetValue(key, out mres))
                    {
                        mres.Set();
                    }
                }
            }
            else
            {
                lock (keyLockObj)
                {
                    mres.Set();
                }
            }
        }

        public static void WaitKey(string key)
        {
            ManualResetEvent mres;
            if (!AlteaCache.keyLocks.TryGetValue(key, out mres))
            {
                lock (keyLockObj)
                {
                    if (AlteaCache.keyLocks.TryGetValue(key, out mres))
                    {
                        mres.WaitOne();
                    }
                }
            }
            else
            {
                lock (keyLockObj)
                {
                    mres.WaitOne();
                }
            }
        }
        #endregion
    }
}