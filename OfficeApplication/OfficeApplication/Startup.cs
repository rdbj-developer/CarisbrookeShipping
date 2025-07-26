using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OfficeApplication.Startup))]
namespace OfficeApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();   // JSL 06/23/2022
        }
    }
}
