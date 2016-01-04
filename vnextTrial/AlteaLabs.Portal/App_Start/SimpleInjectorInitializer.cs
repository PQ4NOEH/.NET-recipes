[assembly: WebActivator.PostApplicationStartMethod(typeof(AlteaLabs.Portal.App_Start.SimpleInjectorInitializer), "Initialize")]

namespace AlteaLabs.Portal.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    using SimpleInjector;
    using SimpleInjector.Extensions;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Models;
    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            
            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
     
        private static void InitializeContainer(Container container)
        {
            container.Register<IUserStore<ApplicationUser>,sdfasfasdfdf>();
            container.Register<IUserRepository, UserToMemory>();

            //#error Register your services here (remove this line).

            // For instance:
            // container.Register<IUserRepository, SqlUserRepository>(Lifestyle.Scoped);
        }
    }
}