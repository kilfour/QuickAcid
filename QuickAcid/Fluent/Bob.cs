namespace QuickAcid.Fluent;


public class SpecBuilder<T>
{
    private readonly Bob<T> bob;
    private readonly string label;

    public SpecBuilder(Bob<T> bob, string label)
    {
        this.label = label;
        this.bob = bob;
    }

    public Bob<Acid> Assert(Func<bool> predicate)
        => bob.Bind(_ => label.Spec(predicate));
}

public class Bob<T>
{
    public readonly QAcidRunner<T> runner;

    public Bob(QAcidRunner<T> runner)
    {
        this.runner = runner;
    }

    // Scoop Muck Lofty Dizzy Roley
    public Bob<TNext> Bind<TNext>(Func<T, QAcidRunner<TNext>> bind)
    {
        var composed =
            from a in runner
            from b in bind(a)
            select b;
        return new Bob<TNext>(composed);
    }

    public Bob<TNext> BindState<TNext>(Func<QAcidState, QAcidRunner<TNext>> bind)
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
    public Bob<Acid> Perform(string label, Action action)
        => Bind(_ => label.Act(action));

    public Bob<Acid> Perform(string label, Func<QAcidContext, Action> effect)
        => BindState(state => label.Act(effect(state)));

    public BobChoiceBuilder<T> Options(Func<Bob<T>, IEnumerable<Bob<Acid>>> choicesBuilder)
    {
        var options = choicesBuilder(this).ToList();
        return new BobChoiceBuilder<T>(this, options);
    }

    public SpecBuilder<T> Spec(string label)
        => new SpecBuilder<T>(this, label);

    public Bob<TNew> TrackedInput<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.TrackedInput(func));

    public Bob<TNew> TrackedInput<TNew>(string label, Func<QAcidContext, TNew> generator)
        => BindState(state => label.TrackedInput(() => generator(state)));

    public Wendy DumpItInAcid()
    {
        var hereYouGo = from _ in runner select Acid.Test;
        return new Wendy(hereYouGo);
    }
}

public class BobChoiceBuilder<T>
{
    private readonly Bob<T> parent;
    private readonly List<Bob<Acid>> options;

    public BobChoiceBuilder(Bob<T> parent, List<Bob<Acid>> options)
    {
        this.parent = parent;
        this.options = options;
    }

    public Bob<Acid> PickOne()
    {
        var combined =
        from _ in parent.runner
        from result in "__00__".Choose(options.Select(opt => opt.runner).ToArray())
        select Acid.Test;

        return new Bob<Acid>(combined);
    }
}