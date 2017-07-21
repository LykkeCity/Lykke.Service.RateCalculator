namespace Lykke.Service.RateCalculator.Core.Domain
{
    public enum OrderAction
    {
        Buy,
        Sell
    }

    public static class OrderActionExt
    {
        public const string Buy = "buy";
        public const string Sell = "sell";

        public static OrderAction ViceVersa(this OrderAction orderAction)
        {
            return orderAction == OrderAction.Buy 
                ? OrderAction.Sell 
                : OrderAction.Buy;
        }
    }
}
