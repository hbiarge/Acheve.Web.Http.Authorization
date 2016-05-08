using System.Web.Http;
using Acheve.Owin.Testing.Security;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using Sample.Api.Controllers;

namespace Sample.Api.IntegrationTests
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            var config = new HttpConfiguration();

            builder.RegisterApiControllers(typeof(ProductsController).Assembly);

            // Configure the api
            Sample.Api.WebApiConfiguration.Configure(config);
            Sample.Api.AutofacConfiguration.Configure(builder);

            // Build the container
            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // --- Register the Owin pipeline ---

            // Autofac
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            // TestServer authentication
            app.UseTestServerAuthentication();

            // WebApi
            app.UseWebApi(config);
        }
    }
}