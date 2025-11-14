namespace QuickAcid.Lab.HorsesForCourses.ValidationHelpers;

public static class Duplicates
{
    private static List<T> GetDuplicates<T>(this IEnumerable<T> collection)
        => [.. collection
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .Distinct()];

    public static bool NoDuplicatesAllowed<T, TException>(this IEnumerable<T> collection)
        where TException : Exception, new()
            => NoDuplicatesAllowed(collection, _ => new TException());

    public static bool NoDuplicatesAllowed<T, TException>(
        this IEnumerable<T> collection,
        Func<IEnumerable<T>, TException> factory)
        where TException : Exception
    {
        var duplicates = collection.GetDuplicates();
        if (duplicates.Count != 0)
            throw factory(duplicates);
        return true;
    }
}