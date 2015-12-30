using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AlteaLabs.Portal.Startup))]
namespace AlteaLabs.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
