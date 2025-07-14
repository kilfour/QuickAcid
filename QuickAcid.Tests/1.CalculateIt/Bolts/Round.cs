namespace LegacyLogic;

public static class Round
{
    public static decimal It(decimal number)
    {
        return Math.Round(number, 2);
    }

    public static decimal AwayFromZero(decimal number)
    {
        return Math.Round(number, 2, MidpointRounding.AwayFromZero);
    }
}
