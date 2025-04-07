using QuickAcid.Nuts;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.FluentContextAware;

public class Bob<T, TContext> // Bob the Architect of Causality
{
    public readonly QAcidRunner<T> runner;

    public Bob(QAcidRunner<T> runner)
    {
        this.runner = runner;
    }

    public Bob<Acid, TContext> ToAcid()
    {
        var mapped =
            from _ in runner
            select Acid.Test;

        return new Bob<Acid, TContext>(mapped);
    }
    internal Bob<TNext, TContext> Bind<TNext>(Func<T, QAcidRunner<TNext>> bind)
    {
        var composed =
            from a in runner
            from b in bind(a)
            select b;
        return new Bob<TNext, TContext>(composed);
    }

    internal Bob<TNext, TContext> BindState<TNext>(Func<QAcidState, QAcidRunner<TNext>> bind)
    {
        QAcidRunner<TNext> composed = state =>
        {
            var result = runner(state);
            if (result.State.Failed)
                return new QAcidResult<TNext>(state, default);

            return bind(result.State)(result.State);
        };
        return new Bob<TNext, TContext>(composed);
    }

    // -------------------------------------------------------------------------
    // register Tracked Input
    //
    public Bob<TNew, TContext> Tracked<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.TrackedInput(func));
    public Bob<TNew, TContext> Tracked<TNew>(QKey<TNew> key, Func<TNew> func)
        => Bind(_ => key.Label.TrackedInput(func));
    // using Context
    public Bob<TNew, TContext> Tracked<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.TrackedInput(() => generator(state)));
    public Bob<TNew, TContext> Tracked<TNew>(QKey<TNew> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.TrackedInput(() => generator(state)));
    // -------------------------------------------------------------------------
    // register Fuzzed Input
    //
    public Bob<TNew, TContext> Fuzzed<TNew>(string label, Generator<TNew> func)
        => Bind(_ => label.ShrinkableInput(func));
    public Bob<TNew, TContext> Fuzzed<TNew>(QKey<TNew> key, Generator<TNew> func)
        => Bind(_ => key.Label.ShrinkableInput(func));
    // using Context
    public Bob<TNew, TContext> Fuzzed<TNew>(string label, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => label.ShrinkableInput(s => generator(state)(s)));
    public Bob<TNew, TContext> Fuzzed<TNew>(QKey<TNew> key, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => key.Label.ShrinkableInput(s => generator(state)(s)));

    // -------------------------------------------------------------------------

    // -------------------------------------------------------------------------
    // Doing Stuff
    //
    public Bob<Acid, TContext> Do(string label, Action action)
       => Bind(_ => label.Act(action));

    public Bob<Acid, TContext> Do(string label, Func<QAcidContext, Action> effect)
        => BindState(state => label.Act(effect(state)));

    public Lofty<T, TContext> As(string label)
        => new(this, label);

    public Trix<T, TContext> Options(Func<Bob<T, TContext>, IEnumerable<Bob<Acid, TContext>>> choicesBuilder)
    {
        var options = choicesBuilder(this).ToList();
        return new Trix<T, TContext>(this, options);
    }

    // -------------------------------------------------------------------------
    // Verifying
    //
    public Bristle<T, TContext> Expect(string label)
        => new(ToAcid(), label);

    public BristlesBrooms<T2, TContext> Expect<T2>(string label, QKey<T2> key)
        => new BristlesBrooms<T2, TContext>(ToAcid(), label, key);

    public Bob<Acid, TContext> Assert(string label, Func<bool> predicate)
        => Bind(_ => label.Spec(predicate));

    public Wendy<TContext> DumpItInAcid()
    {
        var hereYouGo = from _ in runner select Acid.Test;
        return new Wendy<TContext>(hereYouGo);
    }
}

// Character	Role in the show	Role in QuickAcid
// Lofty	 cautious	Conditionally performs actions (OnlyPerform(...).If(...))
// Scoop / Muck	
// Digger / Muck truck	Potential future: mutation, shrinking inspection, failure replay