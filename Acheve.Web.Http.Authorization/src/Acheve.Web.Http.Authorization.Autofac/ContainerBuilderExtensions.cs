using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            var options = new OptionsManager<AuthorizationOptions>(new[]
            {
                new ConfigureOptions<AuthorizationOptions>(optionsConfiguration)
            });

            builder.RegisterInstance(options)
                .As<IOptions<AuthorizationOptions>>()
                .SingleInstance()
                .ExternallyOwned();

            builder.Register(c => new FakeLogger())
                .As<ILogger<DefaultAuthorizationService>>();

            builder.Register(c => new DefaultAuthorizationPolicyProvider(
                c.Resolve<IOptions<AuthorizationOptions>>())).As<IAuthorizationPolicyProvider>();

            builder.Register(c => new DefaultAuthorizationService(
                c.Resolve<IAuthorizationPolicyProvider>(),
                c.Resolve<IEnumerable<IAuthorizationHandler>>(),
                c.Resolve<ILogger<DefaultAuthorizationService>>())).As<IAuthorizationService>();

            builder.Register(c => new PassThroughAuthorizationHandler()).As<IAuthorizationHandler>();
        }
    }
}
