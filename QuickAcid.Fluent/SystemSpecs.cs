using QuickAcid.Bolts;
using QuickAcid.Fluent.Bolts;


namespace QuickAcid.Fluent;

public static class SystemSpecs
{
    public static Bob Define()
    {
        return new Bob(QAcidResult.AcidOnly);
    }
}