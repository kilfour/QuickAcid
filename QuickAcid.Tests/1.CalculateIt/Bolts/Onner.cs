namespace LegacyLogic.Bolts;

public class Onner
{
    private decimal amount;

    private Predicate<decimal>? predicate = null;

    public Onner(decimal amount)
    {
        this.amount = amount;
    }

    public Onner Apply(Func<decimal, decimal> func)
    {
        if (predicate == null)
            amount = func!(amount);
        else if (predicate(amount))
            amount = func!(amount);
        predicate = null;
        return this;
    }

    public Onner Apply(IKnowHowToApplyMyself applier)
    {
        amount = applier.Apply(amount);
        return this;
    }

    public Onner When(Predicate<decimal> predicate)
    {
        this.predicate = predicate;
        return this;
    }

    public decimal Result()
    {
        return amount;
    }
}
