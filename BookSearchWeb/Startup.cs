using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookSearchWeb.Startup))]
namespace BookSearchWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
