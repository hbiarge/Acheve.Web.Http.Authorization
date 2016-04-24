using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Sample.Api
{
    public static class WebApiConfiguration
    {
        public static void Configure(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }
    }
}
