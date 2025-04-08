using QuickAcid.MonadiXEtAl;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Fluent;

public class BristlesBrooms<T>
{
    private readonly Bob bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;
    private readonly Maybe<QKey<T>> key;

    public BristlesBrooms(Bob bob, string label, Maybe<QKey<T>> key = default, Maybe<Func<bool>> iPass = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
        this.iPass = iPass;
    }
    public BroomsWithGate<T> OnlyWhen(Func<T, bool> gateForValue)
    {
        return key.Match(
            some: realKey =>
                new BroomsWithGate<T>(
                    bob,
                    label,
                    state => gateForValue(state.Get(realKey)),
                    key
                ),
            none: () => throw new ThisNotesOnYou("You're singing in the wrong key, buddy.")
        );
    }
    public Bob Ensure(Func<T, bool> mustHold)
    {
        return key.Match2(
            iPass,
            (realKey, gate) => bob.BindState(state =>
                label.SpecIf(gate, () => mustHold(state.Get(realKey)))),
            () => key.Match(
                some: realKey => bob.BindState(state =>
                    label.Spec(() => mustHold(state.Get(realKey)))),
                none: () => throw new InvalidOperationException(
                    $"Ensure<T> was called without UseThe({typeof(T).Name})"))
        );
    }
}

public class BroomsWithGate<T>
{
    private readonly Bob bob;
    private readonly string label;
    private readonly Func<QAcidContext, bool> gate;
    private readonly Maybe<QKey<T>> key;

    public BroomsWithGate(Bob bob, string label, Func<QAcidContext, bool> gate, Maybe<QKey<T>> key)
    {
        this.bob = bob;
        this.label = label;
        this.gate = gate;
        this.key = key;
    }

    public Bob Ensure(Func<T, bool> mustHold)
    {
        return key.Match(
            some: realKey => bob.BindState(state =>
                label.SpecIf(() => gate(state), () => mustHold(state.Get(realKey)))),
            none: () => throw new ThisNotesOnYou("You're singing in the wrong key, buddy.")
        );
    }
}

public class AnotherGate
{
    private readonly Bob bob;
    private readonly string label;
    private readonly Func<QAcidContext, bool> iPass;

    public AnotherGate(Bob bob, string label, Func<QAcidContext, bool> iPass = default)
    {
        this.bob = bob;
        this.label = label;
        this.iPass = iPass;
    }

    public Bob Ensure(Func<QAcidContext, bool> mustHold)
    {
        return bob.BindState(state =>
            label.SpecIf(() => iPass(state), () => mustHold(state)));
    }

    // public Bob Ensure(Func<T, bool> mustHold)
    // {
    //     return key.Match(
    //         some: realKey => bob.BindState(state =>
    //             label.SpecIf(() => iPass(state), () => mustHold(state.Get(realKey)))),
    //         none: () => throw new ThisNotesOnYou("You forgot to call UseThe<T>() before this Ensure.")
    //     );
    // }
}
