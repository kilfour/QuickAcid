namespace QuickAcid.Proceedings;

public class InputDeposition
{
    public string InputLabel { get; }
    public object InputValue { get; }

    public InputDeposition(string inputLabel, object inputValue)
    {
        InputLabel = inputLabel;
        InputValue = inputValue;
    }
};