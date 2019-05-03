using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TryMongoDB.Startup))]
namespace TryMongoDB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
