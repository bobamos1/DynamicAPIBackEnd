using Stripe.Checkout;

namespace APIDynamic
{
    internal class PriceDataOptions : SessionLineItemPriceDataOptions
    {
        public string Currency { get; set; }
        public object ProductData { get; set; }
        public int UnitAmount { get; set; }
    }
}