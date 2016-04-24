using Acheve.Web.Http.Authorization.Infrastructure;

namespace Sample.Api.Infrastructure.Authorization
{
    public static class ProductOperations
    {
        public static readonly OperationAuthorizationRequirement Edit =
            new OperationAuthorizationRequirement { Name = "Edit" };

        public static OperationAuthorizationRequirement GiveDiscount(decimal amount)
        {
            return new DiscountOperationAuthorizationRequirement
            {
                Name = "GiveDiscount",
                Amount = amount
            };
        }
    }
}
