namespace QuickAcid.Reporting;

public static class LabelPrettifier
{
    public static string Prettify(string label)
    {
        if (string.IsNullOrEmpty(label))
            return label;

        var colonIndex = label.IndexOf(':');
        if (colonIndex >= 0)
        {
            // Take only the part before the colon for prettification
            return label.Substring(0, colonIndex);
        }

        return label;
    }
}