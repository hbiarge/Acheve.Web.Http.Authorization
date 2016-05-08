using Autofac;
using Acheve.Web.Http.Authorization.Autofac;
using Microsoft.AspNet.Authorization;
using Sample.Api.Infrastructure.Authorization;
using Sample.Api.Services;

namespace Sample.Api
{
    public static class AutofacConfiguration
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.UsePolicyAuthorization(options =>
            {
                options.AddPolicy(Policies.Sales, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("department", "sales");
                });
                options.AddPolicy(Policies.Over18Years, policy =>
                {
                    policy.AddRequirements(new MinimumAgeRequirement(18));
                });
            });

            // register resource authorization handlers
            builder.Register(c => new ProductAuthorizationHandler(c.Resolve<IDiscountPermissionService>()))
                .As<IAuthorizationHandler>();

            // register services
            builder.Register(c => new InMemoryProductsStore())
                .As<IProductsStore>();
            builder.Register(c => new DefaultDiscountPermissionService())
                .As<IDiscountPermissionService>();
        }
    }
}