using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Heracles.Web.Startup))]
namespace Heracles.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
