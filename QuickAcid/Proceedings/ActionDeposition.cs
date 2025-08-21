namespace QuickAcid.Proceedings;


public record ActionDeposition
{
    public string Label { get; }

    public ActionDeposition(string actionLabel)
    {
        Label = actionLabel;
    }
}
