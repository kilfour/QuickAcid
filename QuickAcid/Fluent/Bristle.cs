using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

// Clean as a whistle Bristle, that's me!
public class Bristle<T>
{
    private readonly Bob<Acid> bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;
    private readonly Maybe<QKey<T>> key;

    public Bristle(Bob<Acid> bob, string label, Maybe<Func<bool>> iPass = default, Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.iPass = iPass;
        this.key = key;
    }

    public Bristle<T> OnlyWhen(Func<bool> iPass)
    => new(bob, label, iPass);

    public AnotherGate<T> OnlyWhen(Func<QAcidContext, bool> iPass)
    => new(bob, label, key, iPass);

    // public Bob<Acid> Do(string label, Action action)
    //    => Bind(_ => label.Act(action));

    // public Bob<Acid> Do(string label, Func<QAcidContext, Action> effect)
    //     => BindState(state => label.Act(effect(state)));

    public Bob<Acid> Ensure(Func<bool> mustHold)
    => iPass.Match(
        some: gate => bob.Bind(_ => label.SpecIf(gate, mustHold)),
        none: () => bob.Bind(_ => label.Spec(mustHold))
    );

    public Bob<Acid> Ensure(Func<QAcidContext, bool> mustHold)
    => iPass.Match(
        some: gate => bob.BindState(state => label.SpecIf(gate, () => mustHold(state))),
        none: () => bob.BindState(state => label.Spec(() => mustHold(state)))
    );

    public BristlesBrooms<T> UseThe(QKey<T> key)
        => new(bob, label, key);

    public BristlesBrooms<T2> UseThe<T2>(QKey<T2> key)
        => new(bob, label, key);
}
