namespace QuickAcid.Proceedings;

public class ExtraDeposition
{
    public string Label { get; }
    public List<string> Messages { get; } = [];

    public ExtraDeposition(string Label)
    {
        this.Label = Label;
    }

    public ExtraDeposition AddMessage(string message)
    {
        Messages.Add(message);
        return this;
    }
}