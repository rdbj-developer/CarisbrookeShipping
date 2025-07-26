using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShipApplication.Startup))]
namespace ShipApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
