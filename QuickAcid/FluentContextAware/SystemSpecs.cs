namespace QuickAcid.FluentContextAware;

public static class SystemSpecs
{
    public static Bob<Acid, TContext> Define<TContext>()
    {
        return new Bob<Acid, TContext>(state => new QAcidResult<Acid>(state, Acid.Test));
    }
}