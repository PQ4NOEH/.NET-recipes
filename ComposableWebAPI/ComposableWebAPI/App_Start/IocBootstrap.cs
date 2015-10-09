using Report.Domain;
using Report.Repository;
using ReportAPI;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;
using System.Web.Mvc;

namespace ComposableWebAPI.App_Start
{
    public class IocBootstrap
    {
        public static void Bootstrap(HttpConfiguration config)
        {
            var container = new SimpleInjector.Container();
            container.Register<IReportRepository<SummaryReport>>( ()=> new ReportRepository<SummaryReport>(), SimpleInjector.Lifestyle.Transient);

            
            container.Register<ISummaryReport>(() => new SummaryReport(container.GetInstance<IReportRepository<SummaryReport>>()), SimpleInjector.Lifestyle.Transient);
            container.RegisterDecorator(typeof(ISummaryReport), typeof(SummaryLogDecorator));
            container.Verify();
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}