namespace Sample.Api.Services
{
    public class DefaultDiscountPermissionService : IDiscountPermissionService
    {
        private const decimal MinAmountAllowed = 0M;
        private const decimal MaxAmountAllowed = 10M;

        public bool IsDiscountAllowed(int productId, decimal amount)
        {
            return amount > MinAmountAllowed && amount <= MaxAmountAllowed;
        }
    }
}
