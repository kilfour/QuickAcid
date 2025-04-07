using QuickAcid;

namespace QuickAcid.FluentContextAware;

// Okey-dokey, J.J.!
public class Trix<T>
{
    private readonly Bob<T> parent;
    private readonly List<Bob<Acid>> options;

    public Trix(Bob<T> parent, List<Bob<Acid>> options)
    {
        this.parent = parent;
        this.options = options;
    }
    public Trix<T> Before(Action preAction)
    {
        var wrapped = options
            .Select(opt =>
                new Bob<Acid>(
                    from _ in parent.runner
                    from __ in "__pre__".Act(preAction)
                    from result in opt.runner
                    select result
                )
            )
            .ToList();

        return new Trix<T>(parent, wrapped);
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