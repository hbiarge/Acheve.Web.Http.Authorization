using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Sample.Api.IntegrationTests
{
    public static class Identities
    {
        public static readonly IEnumerable<Claim> NotSalesUser = new[]
        {
            new Claim(ClaimTypes.Name, "User"),
        };

        public static readonly IEnumerable<Claim> JuniorSalesUser = new[]
        {
            new Claim(ClaimTypes.Name, "JuniorSalesUser"),
            new Claim("department", "sales"),
            new Claim("status", "junior")
        };

        public static readonly IEnumerable<Claim> SeniorSalesUser = new[]
        {
            new Claim(ClaimTypes.Name, "SeniorSalesUser"),
            new Claim("department", "sales"),
            new Claim("status", "senior")
        };

        public static readonly IEnumerable<Claim> Under18User = new[]
        {
            new Claim(ClaimTypes.Name, "Under18User"),
            new Claim(ClaimTypes.DateOfBirth, DateTime.Now.AddYears(-17).ToString(CultureInfo.CurrentCulture) ),
        };

        public static readonly IEnumerable<Claim> Over18User = new[]
        {
            new Claim(ClaimTypes.Name, "Under18User"),
            new Claim(ClaimTypes.DateOfBirth, DateTime.Now.AddYears(-19).ToString(CultureInfo.CurrentCulture) ),
        };
    }
}
