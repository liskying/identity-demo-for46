using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(identity_demo_for46.Startup))]
namespace identity_demo_for46
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
