namespace QuickAcid.Tests._Tools;

public static class RepeatingOnesSelve
{
    public static void Times(this int numberOfTimes, Action action)
    {
        if (numberOfTimes < 0)
            return;
        for (int i = 0; i < numberOfTimes; i++)
            action();
    }

    public static IEnumerable<T> Times<T>(this int numberOfTimes, Func<T> func)
    {
        for (int i = 0; i < numberOfTimes; i++)
            yield return func();
    }

    public static IEnumerable<T> TimesUntil<T>(this int numberOfTimes, Predicate<T> predicate, Func<T> func)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            var result = func();
            yield return result;
            if (predicate(result))
                yield break;
        }
    }
}