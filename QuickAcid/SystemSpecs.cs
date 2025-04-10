namespace QuickAcid;
using QuickAcid.Bolts;

public static class SystemSpecs
{
    public static Fluent.Bob Define()
    {
        return new Fluent.Bob(state => new QAcidResult<Acid>(state, Acid.Test));
    }
}