namespace QuickAcid.Fluent;

public static class SystemSpecs
{
    public static FluentNew.Bob Define()
    {
        return new FluentNew.Bob(state => new QAcidResult<Acid>(state, Acid.Test));
    }

    public static FluentNew.Bob DefineN()
    {
        return new FluentNew.Bob(state => new QAcidResult<Acid>(state, Acid.Test));
    }
}