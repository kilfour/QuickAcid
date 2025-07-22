namespace QuickAcid.TestsDeposition._Tools;

public static class TheJournalist
{
    public static CaseFileInvestigation Unearths(FalsifiableException ex) =>
        new(ex.QAcidReport.CaseFile!);
}
