using System.Web.Http;
using Acheve.Web.Http.Authorization;
using Newtonsoft.Json.Serialization;

namespace Sample.Api
{
    public static class WebApiConfiguration
    {
        public static void Configure(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            // Enable WebApi to understand Asp.Net Core authorization attributes
            config.Filters.Add(new UseAspNetCoreAuthorizationModelAttribute());

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }
    }
}
