using LegacyLogic.Bolts;

namespace LegacyLogic.Discounts;

public class LuckySevenDiscount : IKnowHowToApplyMyself
{
    private readonly int count;

    public LuckySevenDiscount(int count)
    {
        this.count = count;
    }

    public decimal Apply(decimal amount)
    {
        if (EligableForLuckySevenDiscount(amount))
            return ApplyLuckySevenDiscount(amount);
        return amount;
    }
    private bool EligableForLuckySevenDiscount(decimal amount)
    {
        return (int)(amount * 100) % 7 == 0 && count > 5;
    }

    private decimal ApplyLuckySevenDiscount(decimal amount)
    {
        return amount - 0.1m;
    }
}
