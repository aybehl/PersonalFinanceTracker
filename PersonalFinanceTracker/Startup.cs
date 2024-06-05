using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PersonalFinanceTracker.Startup))]
namespace PersonalFinanceTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
