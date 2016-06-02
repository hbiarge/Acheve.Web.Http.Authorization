using System.Web.Http;
using Sample.Api.Infrastructure.Authorization;

namespace Sample.Api.Controllers
{
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {
        // Attribute without policy applies the default policy
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        [Route("defaultPolicy")]
        public IHttpActionResult DefaultPolicy()
        {
            return Ok(new[] { "Value1", "Value2" });
        }

        // Attribute with policy without calculation
        [Microsoft.AspNetCore.Authorization.Authorize(Policies.Sales)]
        [HttpGet]
        [Route("salesPolicy")]
        public IHttpActionResult SalesPolicy()
        {
            return Ok(new[] { "Value1", "Value2" });
        }

        // Attribute with policy with calculations based on user data
        [Microsoft.AspNetCore.Authorization.Authorize(Policies.Over18Years)]
        [HttpGet]
        [Route("over18YearsPolicy")]
        public IHttpActionResult Over18YearsPolicy()
        {
            return Ok(new[] { "Value1", "Value2" });
        }
    }
}
