using QuickAcid.MonadiXEtAl;

namespace QuickAcid.FluentContextAware;

// I can lift it. and Er, yeah, I think so!
public class Lofty<T, TContext>
{
    private readonly Bob<T, TContext> bob;
    private readonly string label;
    private readonly Maybe<QKey<T>> key;

    public Lofty(Bob<T, TContext> bob
        , string label
        , Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
    }

    public LoftysCrane<T, TContext> UseThe(QKey<T> key)
        => new(bob, label, key);

    public Bob<Acid, TContext> Now(Action action)
        => bob.Bind(_ => label.Act(() => action()));
}
