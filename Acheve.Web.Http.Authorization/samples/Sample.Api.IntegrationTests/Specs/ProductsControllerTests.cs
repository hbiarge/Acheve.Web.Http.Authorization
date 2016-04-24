using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Acheve.Owin.Testing.Security;
using Microsoft.Owin.Testing;
using Xunit;

namespace Sample.Api.IntegrationTests.Specs
{
    public class ProductsControllerTests : IDisposable
    {
        private readonly TestServer _server;

        public ProductsControllerTests()
        {
            _server = TestServer.Create<Startup>();
        }

        [Fact]
        public async Task SeniorSales_Can_Get_Products()
        {
            var response = await _server.CreateRequest("products")
                .WithIdentity(Identities.SeniorSalesUser)
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task JuniorSales_Can_Apply_Allowed_Discount_To_Standard_Product()
        {
            // Max allowed discount is 10
            var response = await _server.CreateRequest("products/discount/1/10")
                .WithIdentity(Identities.JuniorSalesUser)
                .PostAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task JuniorSales_Can_Not_Apply_Not_Allowed_Discount_To_Standard_Product()
        {
            // Max allowed discount is 10
            var response = await _server.CreateRequest("products/discount/1/20")
                .WithIdentity(Identities.JuniorSalesUser)
                .PostAsync();

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task JuniorSales_Can_Not_Apply_Allowed_Discount_To_Special_Product()
        {
            // Max allowed discount is 10
            var response = await _server.CreateRequest("products/discount/2/10")
                .WithIdentity(Identities.JuniorSalesUser)
                .PostAsync();

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task SeniorSales_Can_Apply_Allowed_Discount_To_Special_Product()
        {
            // Max allowed discount is 10
            var response = await _server.CreateRequest("products/discount/2/10")
                .WithIdentity(Identities.SeniorSalesUser)
                .PostAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
