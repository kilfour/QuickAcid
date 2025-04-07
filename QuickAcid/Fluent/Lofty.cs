using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

// I can lift it. and Er, yeah, I think so!
public class Lofty<T>
{
    private readonly Bob<T> bob;
    private readonly string label;
    private readonly Maybe<QKey<T>> key;

    public Lofty(Bob<T> bob
        , string label
        , Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
    }

    public LoftysCrane<T> UseThe(QKey<T> key)
        => new(bob, label, key);

    public Bob<Acid> Now(Action action)
        => bob.Bind(_ => label.Act(() => action()));
}
