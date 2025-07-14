using LegacyLogic.Discounts;
using LegacyLogic.Bolts;

namespace LegacyLogic;

public class Calculator
{
    public decimal Total(List<Item> items)
    {
        return GetTotalForItemsWithAmount([.. items.Where(i => i.Number > 0)]);
    }

    private decimal GetTotalForItemsWithAmount(List<Item> items)
    {
        Check.TaxRates(items);
        var articleCountWithoutGiftCards = GetArticleCountWithoutGiftCards(items);
        if (articleCountWithoutGiftCards == 0) return 0;
        var freeItems = GetFreeItems(items);
        return
            On.ThisAmount(GetTotalWithTax(items))
                .Apply(new BogoDiscount(freeItems))
                .Apply(new LuckySevenDiscount(articleCountWithoutGiftCards))
                .Apply(new LoyaltyDiscount(articleCountWithoutGiftCards - freeItems.Count))
                .Apply(new GiftCards(items))
                .Apply(new Shipping(items))
                .Apply(ZeroIsMinimum)
                .Apply(Round.It)
                .Result();
    }

    private static decimal GetTotalWithTax(List<Item> items)
        => items.WithoutGiftCards().Sum(item => Tax.Item(item));

    private static int GetArticleCountWithoutGiftCards(List<Item> items)
        => items.WithoutGiftCards().Select(a => a.Number).Sum();

    private static List<Item> GetFreeItems(List<Item> items)
        => items.OnlyToys()
            .Where(item => item.Number >= 2)
            .OrderBy(item => item.Cost)
            .Take(2)
            .ToList();

    private static decimal ZeroIsMinimum(decimal amount) => Math.Max(amount, 0);
}
