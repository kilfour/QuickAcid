using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

public class Lofty<T>
{
    private readonly Bob<T> bob;
    private readonly string label;
    private readonly Maybe<QKey<T>> key;
    private readonly Maybe<Action<T>> action;

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

    public Bob<Acid> Now(Action<T> effect)
        => bob.BindState(state => key.Match(
            some: realKey => label.Act(() => effect(state.Get(realKey))),
            none: () => throw new ThisNotesOnYou("You're in the wrong key, buddy.")
        ));
    // => bob.BindState(state =>
    //         label.Act(() => effect(state.Get(QKey<T>.New(""))))
    //     );

    //new(bob, label, key, action);

    // public Bob<T> If(Func<QAcidContext, bool> condition)
    // {
    //     return parent.BindState<Acid>(state =>
    //         condition(state)
    //             ? label.Act(action)
    //             : new QAcidResult<T>(state, default(T));
    // }

    //     public Bob<T> If(Func<bool> condition)
    //         => If(_ => condition());
}
