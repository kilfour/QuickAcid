namespace QuickAcid.Tests.Bugs.Horses;

public record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email mag niet leeg zijn.");
        if (!value.Contains("@"))
            throw new ArgumentException("Ongeldig emailformaat.");
        return new EmailAddress(value);
    }
}