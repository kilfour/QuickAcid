using QuickAcid.MonadiXEtAl;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid.Fluent;

// Clean as a whistle Bristle, that's me!
public class Bristle
{
    private readonly Bob bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;


    public Bristle(Bob bob, string label, Maybe<Func<bool>> iPass = default)
    {
        this.bob = bob;
        this.label = label;
        this.iPass = iPass;
    }

    public Bristle OnlyWhen(Func<bool> iPass)
    => new(bob, label, iPass);

    public AnotherGate OnlyWhen(Func<QAcidContext, bool> iPass)
    => new(bob, label, iPass);

    // public Bob Do(string label, Action action)
    //    => Bind(_ => label.Act(action));

    // public Bob Do(string label, Func<QAcidContext, Action> effect)
    //     => BindState(state => label.Act(effect(state)));

    public Bob Ensure(Func<bool> mustHold)
    => iPass.Match(
        some: gate => bob.Bind(_ => label.SpecIf(gate, mustHold)),
        none: () => bob.Bind(_ => label.Spec(mustHold))
    );

    public Bob Ensure(Func<QAcidContext, bool> mustHold)
    => iPass.Match(
        some: gate => bob.BindState(state => label.SpecIf(gate, () => mustHold(state))),
        none: () => bob.BindState(state => label.Spec(() => mustHold(state)))
    );

    public BristlesBrooms<T> UseThe<T>(QKey<T> key)
        => new(bob, label, key);
}
