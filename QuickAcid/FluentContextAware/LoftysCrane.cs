using QuickAcid.MonadiXEtAl;

namespace QuickAcid.FluentContextAware;

public class LoftysCrane<T>
{
    private readonly Bob<T> bob;
    private readonly string label;
    private readonly Maybe<QKey<T>> key;

    public LoftysCrane(Bob<T> bob, string label, Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
    }
    public Bob<Acid> Now(Action<T> effect)
        => bob.BindState(state => key.Match(
            some: realKey => label.Act(() => effect(state.Get(realKey))),
            none: () => throw new ThisNotesOnYou("You're in the wrong key, buddy.")
        ));
}