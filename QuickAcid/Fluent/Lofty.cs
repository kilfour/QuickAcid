using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

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

    public Lofty<T> UseThe(QKey<T> key)
        => new(bob, label, key);

    public Bob<Acid> Now(Action action)
        => bob.Bind(_ => label.Act(() => action()));

    public Bob<Acid> Now(Action<T> effect)
        => bob.BindState(state => key.Match(
            some: realKey => label.Act(() => effect(state.Get(realKey))),
            none: () => throw new ThisNotesOnYou("You're in the wrong key, buddy.")
        ));

    // used intermediate LoftysCrane to split the now
}
