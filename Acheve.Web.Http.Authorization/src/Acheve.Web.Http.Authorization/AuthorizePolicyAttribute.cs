using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;

namespace Acheve.Web.Http.Authorization
{
    public class AuthorizePolicyAttribute : AuthorizationFilterAttribute
    {
        public AuthorizePolicyAttribute()
        {
        }

        public AuthorizePolicyAttribute(string policy)
        {
            Policy = policy;
        }

        public string Policy { get; set; }

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (SkipAuthorization(actionContext))
            {
                return;
            }

            if (await IsAuthorizedAsync(actionContext).ConfigureAwait(false) == false)
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        /// <summary>
        /// Determines whether access for this particular request is authorized. This method uses the user <see cref="IPrincipal"/>
        /// returned via <see cref="HttpRequestContext.Principal"/>.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        /// <returns><c>true</c> if access is authorized; otherwise <c>false</c>.</returns>
        protected virtual async Task<bool> IsAuthorizedAsync(HttpActionContext actionContext)
        {
            IDependencyScope requestScope = actionContext.Request.GetDependencyScope();
            ClaimsPrincipal user = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;

            var authPolicyProvider =
                requestScope.GetService(typeof(IAuthorizationPolicyProvider)) as IAuthorizationPolicyProvider;

            Debug.Assert(authPolicyProvider != null);

            AuthorizationPolicy policyHandler = await authPolicyProvider.GetPolicyAsync(Policy).ConfigureAwait(false);

            Debug.Assert(policyHandler != null);

            var authService =
                requestScope.GetService(typeof(IAuthorizationService)) as IAuthorizationService;

            Debug.Assert(authService != null);

            return await authService.AuthorizeAsync(user, policyHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes requests that fail authorization. This default implementation creates a new response with the
        /// Unauthorized status code. Override this method to provide your own handling for unauthorized requests.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.RequestContext.Principal?.Identity != null
                && actionContext.ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Forbidden");
            }
            else
            {
                actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
            }

        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}