namespace QuickAcid.Reporting;

public class RunReport(string title)
{
    private readonly string title = title;

    public IEnumerable<string> AsStringList()
    {
        return
            [ "----------------------------------------"
            , $"-- {title}"
            , "----------------------------------------"
            , "RUN START :"
            ];
    }
}
