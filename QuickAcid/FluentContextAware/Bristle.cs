using QuickAcid.MonadiXEtAl;

namespace QuickAcid.FluentContextAware;

// Clean as a whistle Bristle, that's me!
public class Bristle<T, TContext>
{
    private readonly Bob<Acid, TContext> bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;
    private readonly Maybe<QKey<T>> key;

    public Bristle(Bob<Acid, TContext> bob, string label, Maybe<Func<bool>> iPass = default, Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.iPass = iPass;
        this.key = key;
    }

    public Bristle<T, TContext> OnlyWhen(Func<bool> iPass)
    => new(bob, label, iPass);

    public Bob<Acid, TContext> Ensure(Func<bool> mustHold)
    => iPass.Match(
        some: gate => bob.Bind(_ => label.SpecIf(gate, mustHold)),
        none: () => bob.Bind(_ => label.Spec(mustHold))
    );

    public Bob<Acid, TContext> Ensure(Func<QAcidContext, bool> mustHold)
    => iPass.Match(
        some: gate => bob.BindState(state => label.SpecIf(gate, () => mustHold(state))),
        none: () => bob.BindState(state => label.Spec(() => mustHold(state)))
    );

    public BristlesBrooms<T, TContext> UseThe(QKey<T> key)
        => new(bob, label, key);

    public BristlesBrooms<T2, TContext> UseThe<T2>(QKey<T2> key)
        => new(bob, label, key);
}
