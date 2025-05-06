using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Fluent.Bolts;

// I can lift it. and Er, yeah, I think so!
public class Lofty
{
    private readonly Bob bob;
    private readonly string label;

    public Lofty(Bob bob
        , string label)
    {
        this.bob = bob;
        this.label = label;
    }

    public LoftysCrane<T> UseThe<T>(QKey<T> key)
        => new LoftysCrane<T>(bob, label, key);

    public Bob Now(Action action)
        => bob.Bind(_ => label.Act(() => action()));

    public Bob Now(Action<QAcidContext> effect)
        => bob.BindState(state => label.Act(() => effect(state)));
}
