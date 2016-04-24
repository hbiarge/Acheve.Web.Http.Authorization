using System;
using System.Security.Claims;
using Acheve.Web.Http.Authorization;

namespace Sample.Api.Infrastructure.Authorization
{
    public class MinimumAgeRequirement
        : AuthorizationHandler<MinimumAgeRequirement>, IAuthorizationRequirement
    {
        private readonly int _minimumAge;

        public MinimumAgeRequirement(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override void Handle(
            AuthorizationContext context,
            MinimumAgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                return;
            }

            var dateOfBirth = Convert.ToDateTime(
                context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value);

            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= _minimumAge)
            {
                context.Succeed(requirement);
            }
        }
    }
}
