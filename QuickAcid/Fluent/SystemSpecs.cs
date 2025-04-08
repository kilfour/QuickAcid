namespace QuickAcid.Fluent;

public static class SystemSpecs
{
    public static Bob<Acid> Define()
    {
        return new Bob<Acid>(state => new QAcidResult<Acid>(state, Acid.Test));
    }

    public static NewBob DefineN()
    {
        return new NewBob(state => new QAcidResult<Acid>(state, Acid.Test));
    }
}