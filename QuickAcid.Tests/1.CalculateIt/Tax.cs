namespace LegacyLogic;

public static class Tax
{
    public static decimal It(decimal exclusive, decimal rate)
    {
        var multiplier = 1 + rate;
        return exclusive * multiplier;
    }

    public static decimal Item(Item item, decimal? total = null)
    {
        decimal? exclusive;
        if (total == null)
            exclusive = item.Cost * item.Number;
        else
            exclusive = total;
        var rate = item.Rate;
        if (item.Import)
            rate += 0.04m;
        else if (item.Category == "Eco" && item.Cost < 20)
            rate /= 2;
        // IT IT ? REALLY ?
        return Round.It(It(exclusive.Value, rate));
    }

    public static decimal GetLowestRate(List<Item> items)
    {
        var lowestTaxRate = 21m;
        foreach (var item in items)
        {
            if (item.Rate < lowestTaxRate) lowestTaxRate = item.Rate;
            if (item.Import == false && item.Category == "Eco" && item.Cost < 20)
            {
                if (item.Rate / 2 < lowestTaxRate) lowestTaxRate = item.Rate / 2;
            }

        }
        return lowestTaxRate;
    }
}
