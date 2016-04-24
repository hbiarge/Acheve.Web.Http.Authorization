using System.Web.Http;
using Acheve.Web.Http.Authorization;
using Sample.Api.Infrastructure.Authorization;

namespace Sample.Api.Controllers
{
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {
        // Attribute without policy applies the default policy: 
        // requires authenticated user
        [AuthorizePolicy]
        [HttpGet]
        [Route("defaultPolicy")]
        public IHttpActionResult DefaultPolicy()
        {
            return Ok(new[] { "Value1", "Value2" });
        }

        // Attribute with policy without calculation
        [AuthorizePolicy(Policies.Sales)]
        [HttpGet]
        [Route("salesPolicy")]
        public IHttpActionResult SalesPolicy()
        {
            return Ok(new[] { "Value1", "Value2" });
        }

        // Attribute with policy with calculations based on user data
        [AuthorizePolicy(Policies.Over18Years)]
        [HttpGet]
        [Route("over18YearsPolicy")]
        public IHttpActionResult Over18YearsPolicy()
        {
            return Ok(new[] { "Value1", "Value2" });
        }
    }
}
