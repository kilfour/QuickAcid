namespace QuickAcid.FluentContextAware;

public static class SystemSpecs
{
    public static Bob<Acid> Define()
    {
        return new Bob<Acid>(state => new QAcidResult<Acid>(state, Acid.Test));
    }
}