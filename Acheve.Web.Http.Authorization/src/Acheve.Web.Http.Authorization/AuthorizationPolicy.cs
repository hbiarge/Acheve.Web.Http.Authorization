// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Acheve.Web.Http.Authorization
{
    public class AuthorizationPolicy
    {
        public AuthorizationPolicy(IEnumerable<IAuthorizationRequirement> requirements, IEnumerable<string> authenticationSchemes)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            if (authenticationSchemes == null)
            {
                throw new ArgumentNullException(nameof(authenticationSchemes));
            }

            if (requirements.Count() == 0)
            {
                throw new InvalidOperationException("AuthorizationPolicy empty");
            }
            Requirements = new List<IAuthorizationRequirement>(requirements).AsReadOnly();
            AuthenticationSchemes = new List<string>(authenticationSchemes).AsReadOnly();
        }

        public IReadOnlyList<IAuthorizationRequirement> Requirements { get; }
        public IReadOnlyList<string> AuthenticationSchemes { get; }

        public static AuthorizationPolicy Combine(params AuthorizationPolicy[] policies)
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            return Combine((IEnumerable<AuthorizationPolicy>)policies);
        }

        public static AuthorizationPolicy Combine(IEnumerable<AuthorizationPolicy> policies)
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            var builder = new AuthorizationPolicyBuilder();
            foreach (var policy in policies)
            {
                builder.Combine(policy);
            }
            return builder.Build();
        }
    }
}