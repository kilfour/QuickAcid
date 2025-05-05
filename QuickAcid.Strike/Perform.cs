namespace QuickAcid.TheFortyNiners;

public static class Perform
{
    public static (string name, Delegate action) This<TModel, TInput>(string name, Action<TModel, TInput> action)
        => (name, action);
}