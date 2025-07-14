namespace LegacyLogic;

public class CalculatorOld
{
    public decimal Total(List<Item> items)
    {
        var ttl = 0m;
        var cnt = 0;
        var lyl = false;
        var ltr = 0.21m;
        foreach (var item in items)
        {
            if (item.Number == 0) continue;
            if (item.Rate != 0.06m && item.Rate != 0.12m && item.Rate != 0.21m)
                throw new ArgumentException($"Invalid tax rate on item: {item.Name}");
            if (item.Rate < ltr) ltr = item.Rate;
            var ln = item.Cost * item.Number;
            if (item.Category != "giftcard")
            {
                var t = item.Rate;
                if (item.Import) { var t2 = t + 0.04m; ln *= 1 + t2; }
                else ln *= 1 + t;
                ln = Math.Round(ln, 2);
                cnt += item.Number;
                ttl += ln;
            }
            if (cnt >= 3 && !lyl) lyl = true;
        }
        var x = (int)(ttl * 100) % 7 == 0 && cnt > 5;
        if (x) ttl -= 0.1m;
        if (lyl) ttl = Math.Round(ttl *= 0.95m, 2);
        if (cnt != 0) foreach (var item in items) if (item.Category == "giftcard") Math.Round(ttl += item.Cost * item.Number, 2, MidpointRounding.AwayFromZero);
        if (ttl < 100)
        {
            var cpn = false; foreach (var item in items) if (item.Name!.ToLower().Contains("freeship")) cpn = true;
            if (!cpn && cnt >= 1) ttl += Math.Round(5m * (1 + ltr), 2);
        }
        if (ttl < 200) { var adj = Math.Min(ttl, 0); if (adj != 0) ttl -= adj; }
        return Math.Round(ttl, 2);
    }
}
