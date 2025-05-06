using QuickAcid.MonadiXEtAl;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Fluent.Bolts;

public class BristlesBroomsOnTheLeft<T>
{
    private readonly Bob bob;
    private readonly string label;
    private readonly Func<QAcidContext, bool> gate;
    private readonly Maybe<QKey<T>> key;

    public BristlesBroomsOnTheLeft(Bob bob, string label, Func<QAcidContext, bool> gate, Maybe<QKey<T>> key)
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
