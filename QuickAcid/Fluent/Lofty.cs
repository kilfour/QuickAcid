using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

// I can lift it. and Er, yeah, I think so!
public class Lofty<T>
{
    private readonly Bob<Acid> bob;
    private readonly string label;
    private readonly Maybe<QKey<T>> key;

    public Lofty(Bob<Acid> bob
        , string label
        , Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
    }

    public LoftysCrane<TNew> UseThe<TNew>(QKey<TNew> key)
        => new LoftysCrane<TNew>(bob, label, key);

    public Bob<Acid> Now(Action action)
        => bob.Bind(_ => label.Act(() => action())).ToAcid();
}
