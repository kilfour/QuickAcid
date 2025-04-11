namespace QuickAcid.Bolts;

public class AlwaysReportedInputsMemory
{
    private Dictionary<string, object> AlwaysReportedInputValues = [];
    private AlwaysReportedInputValueReportPerExecution AlwaysReportedInputReportsPerExecution =
        new AlwaysReportedInputValueReportPerExecution();
}


public class AlwaysReportedInputValueReportPerExecution : IRememberThingsAboutAnExecution
{
    private Dictionary<string, string> AlwaysReportedInputReports { get; set; } = [];

    public void AddReport(string key)
    {
    }
}