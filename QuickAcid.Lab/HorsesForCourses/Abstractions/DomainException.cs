using System.Text.RegularExpressions;

namespace QuickAcid.Lab.HorsesForCourses.Abstractions;

public abstract partial class DomainException : Exception
{
    public DomainException() : base() { }
    public DomainException(string message) : base(message) { }

    public string MessageFromType =>
        LowercaseAllLettersExceptTheFirst(PutASpaceBeforeEachCapital(GetType().Name));

    private static string LowercaseAllLettersExceptTheFirst(string withSpaces)
        => $"{new string([.. withSpaces.Take(1)])}{new string([.. withSpaces.Skip(1).Select(char.ToLower)])}.";

    private static string PutASpaceBeforeEachCapital(string input)
        => MyRegex().Replace(input, " $1");

    [GeneratedRegex("(?<!^)([A-Z])")] // maybe (?<=[a-z0-9])(?=[A-Z])
    private static partial Regex MyRegex();
}
