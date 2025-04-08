using QuickAcid.Nuts;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Fluent;

public class NewBob // Bob the Architect of Causality
{
    public readonly QAcidRunner<Acid> runner;

    public NewBob(QAcidRunner<Acid> runner)
    {
        this.runner = runner;
    }

    internal NewBob Bind<TNext>(Func<Acid, QAcidRunner<TNext>> bind)
    {
        var composed =
            from a in runner
            from b in bind(a)
            select Acid.Test;
        return new NewBob(composed);
    }

    internal NewBob BindState<TNext>(Func<QAcidState, QAcidRunner<TNext>> bind)
    {
        QAcidRunner<TNext> composed =
            state =>
            {
                var result = runner(state);
                if (result.State.Failed)
                    return new QAcidResult<TNext>(state, default(TNext));

                return bind(result.State)(result.State);
            };
        return new NewBob(from _ in composed select Acid.Test);
    }


    // // -------------------------------------------------------------------------
    // // register AlwaysReported Input
    // //
    public NewBob AlwaysReported<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.AlwaysReported(func));
    public NewBob AlwaysReported<TNew>(QKey<TNew> key, Func<TNew> func)
        => Bind(_ => key.Label.AlwaysReported(func));
    public NewBob AlwaysReported<TNew>(QKey<TNew> key, Func<TNew> func, Func<TNew, string> stringify)
        => Bind(_ => key.Label.AlwaysReported(func, stringify));
    // using Context
    public NewBob AlwaysReported<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.AlwaysReported(() => generator(state)));
    public NewBob AlwaysReported<TNew>(QKey<TNew> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.AlwaysReported(() => generator(state)));
    // // -------------------------------------------------------------------------


    // // -------------------------------------------------------------------------
    // // register Fuzzed Input
    // //
    public NewBob Fuzzed<TNew>(string label, Generator<TNew> func)
        => Bind(_ => label.ShrinkableInput(func));
    public NewBob Fuzzed<TNew>(QKey<TNew> key, Generator<TNew> func)
        => Bind(_ => key.Label.ShrinkableInput(func));
    // using Context
    public NewBob Fuzzed<TNew>(string label, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => label.ShrinkableInput(s => generator(state)(s)));
    public NewBob Fuzzed<TNew>(QKey<TNew> key, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => key.Label.ShrinkableInput(s => generator(state)(s)));

    // // -------------------------------------------------------------------------


    // public NewBob Capture<TNew>(QKey<int> key, Func<QAcidContext, TNew> generator)
    //     => BindState(state => key.Label.Input(() => generator(state)));
    // // -------------------------------------------------------------------------
    // // Doing Stuff
    // //
    public NewBob Do(string label, Action action)
       => Bind(_ => label.Act(action));

    public NewBob Do(string label, Action<QAcidContext> effect)
        => BindState(state => label.Act(() => effect(state)));

    // public Lofty<Acid> As(string label)
    //     => new Lofty<Acid>(this, label);

    // public Trix<T> Options(Func<NewBob, IEnumerable<NewBob>> choicesBuilder)
    // {
    //     var options = choicesBuilder(ToAcid()).ToList();
    //     return new Trix<T>(this, options);
    // }

    // // -------------------------------------------------------------------------
    // // Verifying
    // //
    // public Bristle<T> Expect(string label)
    //     => new(ToAcid(), label);

    // public BristlesBrooms<T2> Expect<T2>(string label, QKey<T2> key)
    //     => new BristlesBrooms<T2>(ToAcid(), label, key);

    public NewBob Assert(string label, Func<bool> predicate)
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