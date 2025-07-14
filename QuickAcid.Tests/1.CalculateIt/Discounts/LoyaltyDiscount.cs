using LegacyLogic.Bolts;

namespace LegacyLogic.Discounts;

public class LoyaltyDiscount : IKnowHowToApplyMyself
{
    private readonly int count;

    public LoyaltyDiscount(int count)
    {
        this.count = count;
    }

    public decimal Apply(decimal amount)
    {
        if (EligableForLoyaltyDiscount())
            return ApplyLoyaltyDiscount(amount);
        return amount;
    }

    private bool EligableForLoyaltyDiscount()
    {
        return count >= 3;
    }

    private static decimal ApplyLoyaltyDiscount(decimal amount)
    {
        return Round.It(amount *= 0.95m); ;
    }
}