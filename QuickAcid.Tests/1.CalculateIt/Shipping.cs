using LegacyLogic.Bolts;

namespace LegacyLogic;

public class Shipping : IKnowHowToApplyMyself
{
    private readonly List<Item> items;

    public Shipping(List<Item> items)
    {
        this.items = items;
    }

    public decimal Apply(decimal amount)
    {
        if (amount >= 100)
            return amount;
        if (items.HasFreeShippingCoupon())
            return amount;
        var shippingFee = Tax.It(5m, Tax.GetLowestRate(items));
        amount += Round.It(shippingFee);
        return amount;
    }
}