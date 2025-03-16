using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BJ2247A5.Startup))]

namespace BJ2247A5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
