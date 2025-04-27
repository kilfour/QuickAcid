namespace QuickAcid.Strike;

public static class Test
{
    public static StrikeArrangeBuilder<TModel> This<TModel>(
        Func<TModel> modelFactory,
        Func<TModel, string> stringify = null)
    {
        return new StrikeArrangeBuilder<TModel>(modelFactory, stringify);
    }
}
