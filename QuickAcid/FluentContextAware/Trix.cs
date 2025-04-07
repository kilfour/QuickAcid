using QuickAcid;

namespace QuickAcid.FluentContextAware;

// Okey-dokey, J.J.!
public class Trix<T, TContext>
{
    private readonly Bob<T, TContext> parent;
    private readonly List<Bob<Acid, TContext>> options;

    public Trix(Bob<T, TContext> parent, List<Bob<Acid, TContext>> options)
    {
        this.parent = parent;
        this.options = options;
    }
    public Trix<T, TContext> Before(Action preAction)
    {
        var wrapped = options
            .Select(opt =>
                new Bob<Acid, TContext>(
                    from _ in parent.runner
                    from __ in "__pre__".Act(preAction)
                    from result in opt.runner
                    select result
                )
            )
            .ToList();

        return new Trix<T, TContext>(parent, wrapped);
    }

    public Bob<Acid, TContext> PickOne()
    {
        var combined =
        from _ in parent.runner
        from result in "__00__".Choose(options.Select(opt => opt.runner).ToArray())
        select Acid.Test;
        return new Bob<Acid, TContext>(combined);
    }
}