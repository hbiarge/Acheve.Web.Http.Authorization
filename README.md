# Acheve.Web.Http.Authorization

In WebApi we have been using the Authorize attribute to deny not authenticated users to access the controlers or action methods. In case we wanted fine grained control over authorization with this default attribute we used to specify the Roles or the Users allowed to access the controller or the action method.

But this is not enought in the era of the claims identity. In this days the authorization decisions are made based in a different number of claims presented by the current user. In some cases we need to do operations with the values of the claims to decide if the user is authorized or not.

Moreover, there are situations where we have to take into account not only the information provided by the user, but also the state of the resource you want to access. The type of product, for example.

Those are scenarios that Asp.Net Core has taken into account and are included in the new framework.

This project, allow the developer to use the exact same classes that will use in Asp.Net Core but in WebApi 2.

The big difference is that WebApi 2 don't include a dependency injection component out of the box. So in order to use this project you are forced to plug your preffered dependency injection framework with WebApi 2 and register several types.

For example this is a standard registration with Autofac

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
