using Davalor.SynchronizationManager.Domain;
using Davalor.SynchronizationManager.Domain.Configuration;
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Davalor.SynchronizationManager.Repository
{
    /// <summary>
    /// Factory of existing repositories
    /// </summary>
     [Export(typeof(ISynchroRepositoryFactory))]
    public class SynchroRepositoryFactory : ISynchroRepositoryFactory
    {
        readonly List<RepositoryBuilder> _builders = new List<RepositoryBuilder>();
        readonly IHostConfiguration _hostConfiguration;

        [ImportingConstructor]
        public SynchroRepositoryFactory(IHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            string binPath = System.AppDomain.CurrentDomain.BaseDirectory;
            if(Directory.Exists(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin")))
            {
                binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");
            }
            List<Type> types = new List<Type>();
            foreach (string dll in Directory.GetFiles(binPath, "Davalor.*.dll", SearchOption.AllDirectories))
            {
                try
                {
                   types.AddRange( Assembly.LoadFile(dll).GetTypes());
                }
                catch (FileLoadException loadEx)
                {
                    throw new ConfigurationException("Could not load SynchroRepositoryFactory", loadEx);
                } // The Assembly has already been loaded.
                catch (BadImageFormatException imgEx)
                {
                    throw new ConfigurationException("Could not load SynchroRepositoryFactory", imgEx);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }



            var repositories = types.Where(t => !t.IsAbstract &&
                                                 t.IsClass &&
                                                 t.BaseType != null &&
                                                 t.BaseType.IsGenericType &&
                                                 typeof(GenericDataService<>).Name == t.BaseType.Name).ToList();
            

            var dbContext = types.Where(t => !t.IsAbstract &&
                                                 t.IsClass &&
                                                 t.GetInterfaces().Any(i=> i.IsGenericType && i.Name == typeof(ISynchroDbContext<>).Name)).ToList();
                                                 
            repositories.ForEach(r =>
            {
                var aggregateType = r.BaseType.GetGenericArguments().First();
                var builders = new Dictionary<ESynchroSystem, Func<IHostConfiguration, object>>();
                dbContext.ToList().ForEach(ctx =>
                {
                    var inter = ctx.GetInterfaces().Where(i => i.IsGenericType && i.Name == typeof(ISynchroDbContext<>).Name).First();

                    if (inter.GetGenericArguments().First() == aggregateType)
                    {
                        var synchroSystem = ((dynamic)Activator.CreateInstance(ctx, hostConfiguration)).SynchroSystem;
                        builders.Add(synchroSystem, new Func<IHostConfiguration,object>((IHostConfiguration config) =>
                        {
                            var context = Activator.CreateInstance(ctx, new object[1] { config } );
                            return Activator.CreateInstance(r, new object[1] { context });
                        }));
                    }
                });

                _builders.Add(new RepositoryBuilder(aggregateType, builders));
            });
        }
         /// <summary>
         /// Creates a new instance of a repository which can manage the given aggregate
         /// </summary>
         /// <typeparam name="T">The aggregate type</typeparam>
         /// <returns>The repository instance</returns>
        public ISynchroRepository<T> CreateDataService<T>() where T : class, ISynchroAggregateRoot
        {
            Type searchType = typeof(T);
            return _builders
                    .FirstOrDefault(b => b.AggregateType == searchType)
                    .Builders
                    .First()
                    .Value(_hostConfiguration) as ISynchroRepository<T>;
        }
        /// <summary>
        /// Creates a new instance of a repository which can manage the given aggregate on the given system
        /// </summary>
        /// <typeparam name="T">The aggregate type</typeparam>
        /// <param name="system">The target system.</param>
        /// <returns>The repository instance</returns>
        public ISynchroRepository<T> CreateDataService<T>(ESynchroSystem system) where T : class, ISynchroAggregateRoot
        {
            Type searchType = typeof(T);
            return _builders
                .FirstOrDefault(b => b.AggregateType == searchType)
                .Builders
                .First(i => i.Key == system)
                .Value(_hostConfiguration) as ISynchroRepository<T>;
        }
         /// <summary>
         /// Internal class to store an aggregate type an a collection of posible synchroSystem and factory pair
         /// </summary>
        class RepositoryBuilder
        {
            public readonly Type AggregateType;
            public readonly Dictionary<ESynchroSystem, Func<IHostConfiguration, object>> Builders;
            public RepositoryBuilder(Type aggregateType, Dictionary<ESynchroSystem, Func<IHostConfiguration,object>> builders)
            {
                AggregateType = aggregateType;
                Builders = builders;
            }
        }
    }
}
