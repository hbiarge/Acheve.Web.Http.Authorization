using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Sample.Api.Infrastructure.Authorization
{
    public class DiscountOperationAuthorizationRequirement : OperationAuthorizationRequirement
    {
        public decimal Amount { get; set; }
    }
}