using QuickAcid.Nuts;

namespace QuickAcid.Fluent;

public class Bob<T> // Bob the Architect of Causality
{
    public readonly QAcidRunner<T> runner;

    public Bob(QAcidRunner<T> runner)
    {
        this.runner = runner;
    }

    internal Bob<TNext> Bind<TNext>(Func<T, QAcidRunner<TNext>> bind)
    {
        var composed =
            from a in runner
            from b in bind(a)
            select b;
        return new Bob<TNext>(composed);
    }

    internal Bob<TNext> BindState<TNext>(Func<QAcidState, QAcidRunner<TNext>> bind)
    {
        QAcidRunner<TNext> composed = state =>
        {
            var result = runner(state);
            if (result.State.Failed)
                return new QAcidResult<TNext>(state, default(TNext));

            return bind(result.State)(result.State);
        };
        return new Bob<TNext>(composed);
    }

    // -------------------------------------------------------------------------
    // register Tracked Input
    //
    public Bob<TNew> Tracked<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.TrackedInput(func));
    public Bob<TNew> Tracked<TNew>(QKey<TNew> key, Func<TNew> func)
        => Bind(_ => key.Label.TrackedInput(func));
    // using Context
    public Bob<TNew> Tracked<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.TrackedInput(() => generator(state)));
    public Bob<TNew> Tracked<TNew>(QKey<TNew> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.TrackedInput(() => generator(state)));

    // -------------------------------------------------------------------------
    // Doing Stuff
    //
    public Bob<Acid> Do(string label, Action action)
       => Bind(_ => label.Act(action));

    public Bob<Acid> Do(string label, Func<QAcidContext, Action> effect)
        => BindState(state => label.Act(effect(state)));

    public Lofty<T> As(string label)
        => new(this, label);

    public Trix<T> Options(Func<Bob<T>, IEnumerable<Bob<Acid>>> choicesBuilder)
    {
        var options = choicesBuilder(this).ToList();
        return new Trix<T>(this, options);
    }

    // -------------------------------------------------------------------------
    // Verifying
    //
    public Bristle<T> Expect(string label)
        => new(this, label);

    public Bob<Acid> Assert(string label, Func<bool> predicate)
        => Bind(_ => label.Spec(predicate));

    public Wendy DumpItInAcid()
    {
        var hereYouGo = from _ in runner select Acid.Test;
        return new Wendy(hereYouGo);
    }
}

// Character	Role in the show	Role in QuickAcid
// Lofty	 cautious	Conditionally performs actions (OnlyPerform(...).If(...))
// Scoop / Muck	
// Digger / Muck truck	Potential future: mutation, shrinking inspection, failure replay