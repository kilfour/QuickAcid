namespace QuickAcid.Fluent;
using QuickAcid.Nuts.Bolts;

// Okey-dokey, J.J.!
public class Trix
{
    private readonly Bob parent;
    private readonly List<Bob> options;

    public Trix(Bob parent, List<Bob> options)
    {
        this.parent = parent;
        this.options = options;
    }

    public Bob PickOne()
    {
        var combined =
        from _ in parent.runner
        from result in "__00__".Choose(options.Select(opt => opt.runner).ToArray())
        select Acid.Test;
        return new Bob(combined);
    }
}