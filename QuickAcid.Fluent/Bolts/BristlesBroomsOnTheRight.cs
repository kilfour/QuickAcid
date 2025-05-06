using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Fluent.Bolts;

public class BristlesBroomsOnTheRight
{
    private readonly Bob bob;
    private readonly string label;
    private readonly Func<QAcidContext, bool> iPass;

    public BristlesBroomsOnTheRight(Bob bob, string label, Func<QAcidContext, bool> iPass = default!)
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
}
