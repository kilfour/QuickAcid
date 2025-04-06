namespace QuickAcid.Fluent;

public class BobChoiceBuilder<T>
{
    private readonly Bob<T> parent;
    private readonly List<Bob<Acid>> options;

    public BobChoiceBuilder(Bob<T> parent, List<Bob<Acid>> options)
    {
        this.parent = parent;
        this.options = options;
    }

    public Bob<Acid> PickOne()
    {
        var combined =
        from _ in parent.runner
        from result in "__00__".Choose(options.Select(opt => opt.runner).ToArray())
        select Acid.Test;

        return new Bob<Acid>(combined);
    }
}