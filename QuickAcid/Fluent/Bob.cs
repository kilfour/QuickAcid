namespace QuickAcid.Fluent;

public class Bob<T>
{
    protected readonly QAcidRunner<T> runner;

    public Bob(QAcidRunner<T> runner)
    {
        this.runner = runner;
    }

    private Bob<TNext> Bind<TNext>(Func<T, QAcidRunner<TNext>> bind)
    {
        var composed =
            from a in runner
            from b in bind(a)
            select b;
        return new Bob<TNext>(composed);
    }

    public Bob<Acid> Perform(string label, Action action)
        => Bind(_ => label.Act(action));

    public Bob<TNew> TrackedInput<TNew>(string label, Func<TNew> func)
        => Bind(_ => label.TrackedInput(func));

    public Wendy DumpItInAcid()
    {
        var hereYouGo = from _ in runner select Acid.Test;
        return new Wendy(hereYouGo);
    }
}

