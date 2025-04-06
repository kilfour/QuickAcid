using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Fluent;

// Clean as a whistle Bristle, that's me!
public class Bristle<T>
{
    private readonly Bob<T> bob;
    private readonly string label;
    private readonly Maybe<Func<bool>> iPass;

    public Bristle(Bob<T> bob, string label, Maybe<Func<bool>> iPass = default)
    {
        this.bob = bob;
        this.label = label;
        this.iPass = iPass;
    }

    public Bristle(Bob<T> bob, string label)
    {
        this.label = label;
        this.bob = bob;
    }

    public Bristle<T> OnlyWhen(Func<bool> iPass)
    => new(bob, label, iPass);

    public Bob<Acid> Assert(Func<bool> mustHold)
    => iPass.Match(
        some: gate => bob.Bind(_ => label.SpecIf(gate, mustHold)),
        none: () => bob.Bind(_ => label.Spec(mustHold))
    );
}
