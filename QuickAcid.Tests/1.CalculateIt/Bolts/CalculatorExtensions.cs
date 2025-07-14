namespace LegacyLogic.Bolts;

public static class CalculatorExtensions
{
    public static IEnumerable<Item> OnlyToys(this IEnumerable<Item> items)
    {
        return items.Where(a => a.Category == "toys");
    }

    public static IEnumerable<Item> OnlyGiftCards(this IEnumerable<Item> items)
    {
        return items.Where(a => a.Category == "giftcard");
    }

    public static IEnumerable<Item> WithoutGiftCards(this IEnumerable<Item> items)
    {
        return items.Where(a => a.Category != "giftcard");
    }

    public static bool HasFreeShippingCoupon(this IEnumerable<Item> items)
    {
        return items.Any(a => a.Name!.Contains("freeship", StringComparison.CurrentCultureIgnoreCase));
    }
}
