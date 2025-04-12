using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Fluent;

// The Architect of Causality
public class Bob
{
    public readonly QAcidRunner<Acid> runner;

    public Bob(QAcidRunner<Acid> runner)
    {
        this.runner = runner;
    }

    internal Bob Bind<TNext>(Func<Acid, QAcidRunner<TNext>> bind)
    {
        var composed =
            from a in runner
            from b in bind(a)
            select Acid.Test;
        return new Bob(composed);
    }

    internal Bob BindState<TNext>(Func<QAcidState, QAcidRunner<TNext>> bind)
    {
        QAcidRunner<TNext> composed =
            state =>
            {
                var result = runner(state);
                if (result.State.CurrentContext.Failed)
                    return QAcidResult.None<TNext>(state);

                return bind(result.State)(result.State);
            };
        return new Bob(from _ in composed select Acid.Test);
    }


    // -------------------------------------------------------------------------
    // register AlwaysReported Input
    //
    public Bob AlwaysReported<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.AlwaysReported(func));
    public Bob AlwaysReported<TNew>(QKey<TNew> key, Func<TNew> func)
        => Bind(_ => key.Label.AlwaysReported(func));
    public Bob AlwaysReported<TNew>(QKey<TNew> key, Func<TNew> func, Func<TNew, string> stringify)
        => Bind(_ => key.Label.AlwaysReported(func, stringify));
    // using Context
    public Bob AlwaysReported<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.AlwaysReported(() => generator(state)));
    public Bob AlwaysReported<TNew>(QKey<TNew> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.AlwaysReported(() => generator(state)));
    // -------------------------------------------------------------------------


    // -------------------------------------------------------------------------
    // register Fuzzed Input
    //
    public Bob Fuzzed<TNew>(string label, Generator<TNew> func)
        => Bind(_ => label.ShrinkableInput(func));
    public Bob Fuzzed<TNew>(QKey<TNew> key, Generator<TNew> func)
        => Bind(_ => key.Label.ShrinkableInput(func));
    // using Context
    public Bob Fuzzed<TNew>(string label, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => label.ShrinkableInput(s => generator(state)(s)));
    public Bob Fuzzed<TNew>(QKey<TNew> key, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => key.Label.ShrinkableInput(s => generator(state)(s)));

    // -------------------------------------------------------------------------

    // -------------------------------------------------------------------------
    // Capturing Stuff
    //
    public Bob Capture<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.Capture(() => generator(state)));
    public Bob Capture<TNew>(QKey<int> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.Capture(() => generator(state)));


    // -------------------------------------------------------------------------
    // Doing Stuff
    //
    public Bob Do(string label, Action action)
       => Bind(_ => label.Act(action));

    public Bob Do(string label, Action<QAcidContext> effect)
        => BindState(state => label.Act(() => effect(state)));

    public Lofty As(string label)
        => new Lofty(this, label);

    public Trix Options(Func<Bob, IEnumerable<Bob>> choicesBuilder)
    {
        var options = choicesBuilder(this).ToList();
        return new Trix(this, options);
    }

    // -------------------------------------------------------------------------
    // Verifying
    //
    public Bristle Expect(string label)
        => new(this, label);

    public BristlesBrooms<T> Expect<T>(string label, QKey<T> key)
        => new BristlesBrooms<T>(this, label, key);

    public Bob Assert(string label, Func<bool> predicate)
        => Bind(_ => label.Spec(predicate));

    public Wendy DumpItInAcid()
    {
        return new Wendy(runner);
    }
}

// Character	Role in the show	Role in QuickAcid
// Lofty	 cautious	Conditionally performs actions (OnlyPerform(...).If(...))
// Scoop / Muck	
// Digger / Muck truck	Potential future: mutation, shrinking inspection, failure replay