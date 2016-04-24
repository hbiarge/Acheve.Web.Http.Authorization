using System;
using System.Net;
using System.Threading.Tasks;
using Acheve.Owin.Testing.Security;
using Microsoft.Owin.Testing;
using Xunit;

namespace Sample.Api.IntegrationTests.Specs
{
    public class VauesControllerTests : IDisposable
    {
        private readonly TestServer _server;

        public VauesControllerTests()
        {
            _server = TestServer.Create<Startup>();
        }

        [Fact]
        public async Task DefaultPolicy_Not_Authenticated_NotAuthorized()
        {
            var response = await _server.CreateRequest("values/defaultPolicy")
                .GetAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DefaultPolicy_Authenticated_Ok()
        {
            var response = await _server.CreateRequest("values/defaultPolicy")
                .WithIdentity(Identities.NotSalesUser)
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SalesPolicy_Not_Authenticated_Unauthorized()
        {
            var response = await _server.CreateRequest("values/salesPolicy")
                .GetAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SalesPolicy_Authenticated_Without_Requirements_Forbidden()
        {
            var response = await _server.CreateRequest("values/salesPolicy")
                .WithIdentity(Identities.NotSalesUser)
                .GetAsync();

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task SalesPolicy_Authenticated_With_Requirements_Ok()
        {
            var response = await _server.CreateRequest("values/salesPolicy")
                .WithIdentity(Identities.JuniorSalesUser)
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Over18YearsPolicy_Not_Authenticated_Unauthorized()
        {
            var response = await _server.CreateRequest("values/over18YearsPolicy")
                .GetAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Over18YearsPolicy_Authenticated_Without_Requirements_Forbidden()
        {
            var response = await _server.CreateRequest("values/over18YearsPolicy")
                .WithIdentity(Identities.Under18User)
                .GetAsync();

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Over18YearsPolicy_Authenticated_With_Requirements_Ok()
        {
            var response = await _server.CreateRequest("values/over18YearsPolicy")
                .WithIdentity(Identities.Over18User)
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
