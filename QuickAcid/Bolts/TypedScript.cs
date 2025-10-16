using System.Text.RegularExpressions;
using QuickAcid;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Bolts;

public record TypedScript
{
    public static string LabelFromType(Type type)
    {
        IEnumerable<string> names = [FormatTypeName(type)];
        Type? parent = type.DeclaringType;
        while (parent != null && typeof(TypedScript).IsAssignableFrom(parent))
        {
            names = names.Prepend(FormatTypeName(parent));
            parent = parent.DeclaringType;

        }
        return string.Join(" ", names);
    }


    public static string FormatTypeName(Type type) =>
        LowercaseAllLettersExceptTheFirst(PutASpaceBeforeEachCapital(type.Name));

    private static string LowercaseAllLettersExceptTheFirst(string withSpaces)
        => $"{new string([.. withSpaces.Take(1)])}{new string([.. withSpaces.Skip(1).Select(char.ToLower)])}";

    private static string PutASpaceBeforeEachCapital(string input)
        => Regex.Replace(input, "(?<!^)([A-Z])", " $1");
}