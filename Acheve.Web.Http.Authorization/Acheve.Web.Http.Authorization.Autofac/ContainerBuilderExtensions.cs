using System;
using System.Collections.Generic;
using Acheve.Web.Http.Authorization.Infrastructure;
using Autofac;

namespace Acheve.Web.Http.Authorization.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void UsePolicyAuthorization(this ContainerBuilder builder, Action<AuthorizationOptions> optionsConfiguration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (optionsConfiguration == null)
            {
                throw new ArgumentNullException(nameof(optionsConfiguration));
            }

            var authorizationOptions = new AuthorizationOptions();
            optionsConfiguration(authorizationOptions);

            builder.RegisterInstance(authorizationOptions)
                .As<AuthorizationOptions>()
                .SingleInstance()
                .ExternallyOwned();

            builder.Register(c => new DefaultAuthorizationPolicyProvider(
                c.Resolve<AuthorizationOptions>())).As<IAuthorizationPolicyProvider>();

            builder.Register(c => new DefaultAuthorizationService(
                c.Resolve<IAuthorizationPolicyProvider>(),
                c.Resolve<IEnumerable<IAuthorizationHandler>>())).As<IAuthorizationService>();

            builder.Register(c => new PassThroughAuthorizationHandler()).As<IAuthorizationHandler>();
        }
    }
}
