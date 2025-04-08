using QuickAcid.MonadiXEtAl;
using QuickAcid.Nuts;

namespace QuickAcid.Fluent;

public class LoftysCrane<T>
{
    private readonly Bob<Acid> bob;
    private readonly string label;
    private readonly Maybe<QKey<T>> key;

    public LoftysCrane(Bob<Acid> bob, string label, Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
    }
    public Bob<Acid> Now(Action<T> effect)
        => bob.BindState(state => key.Match(
            some: realKey => label.Act(() => effect(state.Get(realKey))),
            none: () => throw new ThisNotesOnYou("You're in the wrong key, buddy.")
        )).ToAcid();
}