using LegacyLogic.Bolts;

namespace LegacyLogic.Discounts;

public class GiftCards : IKnowHowToApplyMyself
{
    private readonly List<Item> items;

    public GiftCards(List<Item> items)
    {
        this.items = items;
    }

    public decimal Apply(decimal amount)
    {
        return items
            .OnlyGiftCards()
            .Select(a => Round.AwayFromZero(amount += a.Cost * a.Number))
            .LastOrDefault(amount);
    }
}
