namespace QuickAcid.Fluent;

public interface QAcidContext
{
    T Get<T>(string key);
}

public static class SystemSpecs
{
    public static Bob<Acid> Define()
    {
        return new Bob<Acid>(state => new QAcidResult<Acid>(state, Acid.Test));
    }
}