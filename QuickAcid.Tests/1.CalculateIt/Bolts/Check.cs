namespace LegacyLogic.Bolts;

public static class Check
{
    public static void TaxRates(List<Item> items)
    {
        items.ForEach(CheckRate);
    }
    private static void CheckRate(Item item)
    {
        if (item.Number == 0) return;
        if (item.Rate != 0.06m && item.Rate != 0.12m && item.Rate != 0.21m)
            throw new ArgumentException($"Invalid tax rate on item: {item.Name}");
    }
}
