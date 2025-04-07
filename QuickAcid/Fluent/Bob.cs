using QuickAcid.Nuts;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Fluent;

public class Bob<T> // Bob the Architect of Causality
{
    public readonly QAcidRunner<T> runner;

    public Bob(QAcidRunner<T> runner)
    {
        this.runner = runner;
    }
    public Bob<Acid> ToAcid()
    {
        var mapped =
            from _ in runner
            select Acid.Test;

        return new Bob<Acid>(mapped);
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
    // register AlwaysReported Input
    //
    public Bob<TNew> AlwaysReported<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.AlwaysReported(func));
    public Bob<TNew> AlwaysReported<TNew>(QKey<TNew> key, Func<TNew> func)
        => Bind(_ => key.Label.AlwaysReported(func));
    // using Context
    public Bob<TNew> AlwaysReported<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.AlwaysReported(() => generator(state)));
    public Bob<TNew> AlwaysReported<TNew>(QKey<TNew> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.AlwaysReported(() => generator(state)));
    // -------------------------------------------------------------------------
    // register Fuzzed Input
    //
    public Bob<TNew> Fuzzed<TNew>(string label, Generator<TNew> func)
        => Bind(_ => label.ShrinkableInput(func));
    public Bob<TNew> Fuzzed<TNew>(QKey<TNew> key, Generator<TNew> func)
        => Bind(_ => key.Label.ShrinkableInput(func));
    // using Context
    public Bob<TNew> Fuzzed<TNew>(string label, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => label.ShrinkableInput(s => generator(state)(s)));
    public Bob<TNew> Fuzzed<TNew>(QKey<TNew> key, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => key.Label.ShrinkableInput(s => generator(state)(s)));

    // -------------------------------------------------------------------------

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
        => new(ToAcid(), label);

    public BristlesBrooms<T2> Expect<T2>(string label, QKey<T2> key)
        => new BristlesBrooms<T2>(ToAcid(), label, key);

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