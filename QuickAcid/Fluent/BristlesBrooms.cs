using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

public class BristlesBrooms<T>
{
    private readonly Bob<T> bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;
    private readonly Maybe<QKey<T>> key;

    public BristlesBrooms(Bob<T> bob, string label, Maybe<QKey<T>> key = default, Maybe<Func<bool>> iPass = default)
    {
        this.bob = bob;
        this.label = label;
        this.key = key;
        this.iPass = iPass;
    }

    public Bob<Acid> Ensure(Func<T, bool> mustHold)
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