namespace LegacyLogic.Bolts;

public static class On
{
    public static Onner ThisAmount(decimal amount)
    {
        return new Onner(amount);
    }
}
