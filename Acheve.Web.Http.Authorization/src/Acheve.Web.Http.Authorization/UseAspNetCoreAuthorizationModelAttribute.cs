using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Acheve.Web.Http.Authorization
{
    public class UseAspNetCoreAuthorizationModelAttribute : AuthorizationFilterAttribute
    {
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
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            IDependencyScope requestScope = actionContext.Request.GetDependencyScope();
            IOptions<AuthorizationOptions> authorizationOptions =
                requestScope.GetService(typeof(IOptions<AuthorizationOptions>)) as IOptions<AuthorizationOptions>;

            if (authorizationOptions == null)
            {
                return false;
            }

            var controllerAuthorizeData = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<IAuthorizeData>();
            var actionAuthorizeData = actionContext.ActionDescriptor.GetCustomAttributes<IAuthorizeData>();

            AuthorizationPolicy policy = AuthorizationPolicy.Combine(
                    authorizationOptions.Value,
                    controllerAuthorizeData.Concat(actionAuthorizeData));

            ClaimsPrincipal user = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;
            var authService = requestScope.GetService(typeof(IAuthorizationService)) as IAuthorizationService;

            if (authService == null)
            {
                return false;
            }

            // Build a ClaimsPrincipal with the Policy's required authentication types
            if (policy.AuthenticationSchemes != null && policy.AuthenticationSchemes.Any())
            {
                ClaimsPrincipal newPrincipal = null;
                foreach (var scheme in policy.AuthenticationSchemes)
                {
                    var result = await actionContext.Request.GetOwinContext().Authentication.AuthenticateAsync(scheme);
                    if (result != null)
                    {
                        newPrincipal = MergeUserPrincipal(newPrincipal, result.Identity);
                    }
                }

                // If all schemes failed authentication, provide a default identity anyways
                if (newPrincipal == null)
                {
                    newPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                }

                actionContext.ControllerContext.RequestContext.Principal = newPrincipal;
            }

            return user != null
                   && user.Identities.Any(i => i.IsAuthenticated)
                   && await authService.AuthorizeAsync(user, policy);

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
            return actionContext.ActionDescriptor.GetCustomAttributes<IAllowAnonymous>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<IAllowAnonymous>().Any();
        }

        private static ClaimsPrincipal MergeUserPrincipal(ClaimsPrincipal existingPrincipal, ClaimsIdentity additionalIdentity)
        {
            var newPrincipal = new ClaimsPrincipal();

            // New principal identities go first
            if (additionalIdentity != null)
            {
                newPrincipal.AddIdentity(additionalIdentity);
            }

            // Then add any existing non empty or authenticated identities
            if (existingPrincipal != null)
            {
                newPrincipal.AddIdentities(existingPrincipal.Identities.Where(i => i.IsAuthenticated || i.Claims.Any()));
            }
            return newPrincipal;
        }
    }
}