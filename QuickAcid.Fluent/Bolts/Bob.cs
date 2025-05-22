using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Fluent.Bolts;

// The Architect of Causality
public class Bob
{
    public readonly QAcidScript<Acid> script;

    public Bob(QAcidScript<Acid> script)
    {
        this.script = script;
    }

    internal Bob Bind<TNext>(Func<Acid, QAcidScript<TNext>> bind)
    {
        var composed =
            from a in script
            from b in bind(a)
            select Acid.Test;
        return new Bob(composed);
    }

    internal Bob BindState<TNext>(Func<QAcidState, QAcidScript<TNext>> bind)
    {
        QAcidScript<TNext> composed =
            state =>
            {
                var result = script(state);
                if (result.State.CurrentContext.Failed)
                    return QAcidResult.None<TNext>(state);

                return bind(result.State)(result.State);
            };
        return new Bob(from _ in composed select Acid.Test);
    }


    // -------------------------------------------------------------------------
    // register Tracked Input
    //
    public Bob Tracked<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.Tracked(func));
    public Bob Tracked<TNew>(QKey<TNew> key, Func<TNew> func)
        => Bind(_ => key.Label.Tracked(func));
    public Bob Tracked<TNew>(string label, Func<TNew> func, Func<TNew, string> stringify)
        => Bind(_ => label.Tracked(func, stringify));
    public Bob Tracked<TNew>(QKey<TNew> key, Func<TNew> func, Func<TNew, string> stringify)
        => Bind(_ => key.Label.Tracked(func, stringify));
    // using Context
    public Bob Tracked<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.Tracked(() => generator(state)));
    public Bob Tracked<TNew>(QKey<TNew> key, Func<QAcidContext, TNew> generator)
        => BindState(state => key.Label.Tracked(() => generator(state)));
    // -------------------------------------------------------------------------


    // -------------------------------------------------------------------------
    // register Fuzzed Input
    //
    public Bob Fuzzed<TNew>(string label, Generator<TNew> func)
        => Bind(_ => label.Input(func));
    public Bob Fuzzed<TNew>(QKey<TNew> key, Generator<TNew> func)
        => Bind(_ => key.Label.Input(func));
    // using Context
    public Bob Fuzzed<TNew>(string label, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => label.Input(s => generator(state)(s)));
    public Bob Fuzzed<TNew>(QKey<TNew> key, Func<QAcidContext, Generator<TNew>> generator)
        => BindState(state => key.Label.Input(s => generator(state)(s)));
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

    public Bob Options(Func<Bob, IEnumerable<Bob>> choicesBuilder)
    {
        var options = choicesBuilder(this).Select(opt => opt.script).ToArray();
        var combined =
            from _ in script
            from result in "__00__".Choose(options) // $"__choose__{Guid.NewGuid()}"
            select Acid.Test;
        return new Bob(combined);
    }

    // -------------------------------------------------------------------------
    // Verifying
    //
    public Bristle Expect(string label)
        => new(this, label);

    public BristlesBrooms<T> Expect<T>(string label, QKey<T> key)
        => new BristlesBrooms<T>(this, label, key);

    public Bob Assert(string label, Func<QAcidContext, bool> predicate)
        => BindState(state => label.Spec(() => predicate(state)));

    public Bob Assay(string label, Func<QAcidContext, bool> predicate)
        => BindState(state => label.Assay(() => predicate(state)));

    public Wendy DumpItInAcid()
    {
        return new Wendy(script);
    }
}