using LegacyLogic.Bolts;

namespace LegacyLogic.Discounts;

public class BogoDiscount : IKnowHowToApplyMyself
{
    private readonly List<Item> freeItems;

    public BogoDiscount(List<Item> freeItems)
    {
        this.freeItems = freeItems;
    }

    public decimal Apply(decimal amount)
    {
        if (freeItems.Count == 0) return amount;
        if (freeItems.Count >= 2)
        {
            freeItems.Sort((x, y) => x.Cost.CompareTo(y.Cost));
            amount -= Tax.Item(freeItems[0], freeItems[0].Cost);
            amount -= Tax.Item(freeItems[1], freeItems[1].Cost);
        }
        else if (freeItems.Count == 1)
        {
            amount -= Tax.Item(freeItems[0], freeItems[0].Cost);
        }
        return amount;
    }
}
