using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Connect.Startup))]
namespace Connect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
