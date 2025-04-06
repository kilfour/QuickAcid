using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

// Clean as a whistle Bristle, that's me!
public class Bristle<T>
{
    private readonly Bob<T> bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;
    private readonly Maybe<QKey<T>> key;

    public Bristle(Bob<T> bob, string label, Maybe<Func<bool>> iPass = default, Maybe<QKey<T>> key = default)
    {
        this.bob = bob;
        this.label = label;
        this.iPass = iPass;
        this.key = key;
    }

    public Bristle<T> OnlyWhen(Func<bool> iPass)
    => new(bob, label, iPass);


    // MustHold ?
    public Bob<Acid> Assert(Func<bool> mustHold)
    => iPass.Match(
        some: gate => bob.Bind(_ => label.SpecIf(gate, mustHold)),
        none: () => bob.Bind(_ => label.Spec(mustHold))
    );


    public Bob<Acid> Assert(Func<QAcidContext, bool> mustHold)
    => iPass.Match(
        some: gate => bob.BindState(state => label.SpecIf(gate, () => mustHold(state))),
        none: () => bob.BindState(state => label.Spec(() => mustHold(state)))
    );

    public Bob<Acid> Assert(Func<T, bool> mustHold)
    {
        return
            key.Match(
                some: realKey => iPass.Match(
                    some: gate => bob.BindState(state => label.SpecIf(gate, () => mustHold(state.Get(realKey)))),
                    none: () => bob.BindState(state => label.Spec(() => mustHold(state.Get(realKey))))
                ),
                none: () => throw new InvalidOperationException(
                        $"Assert<T> was called on Bristle without a matching UseThe({typeof(T).Name})"));

        // if (key.TryGet(out var realKey))
        // {
        //     return iPass.Match(
        //         some: gate => bob.BindState(state => label.SpecIf(gate, () => mustHold(state.Get(realKey)))),
        //         none: () => bob.BindState(state => label.Spec(() => mustHold(state.Get(realKey))))
        //     );
        // }

        // throw new InvalidOperationException(
        //     $"Assert<T> was called on Bristle without a matching UseThe({typeof(T).Name})"
        // );
    }
}
