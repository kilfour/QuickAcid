namespace QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.Fluent.Bolts;

public static class SystemSpecs
{
    public static Bob Define()
    {
        return new Bob(QAcidResult.AcidOnly);
    }
}