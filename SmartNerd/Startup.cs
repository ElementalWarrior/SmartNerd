using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartNerd.Startup))]
namespace SmartNerd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
