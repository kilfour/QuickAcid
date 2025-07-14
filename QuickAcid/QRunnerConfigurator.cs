namespace QuickAcid;


public class QRunnerConfigurator
{
    private QAcidScript<Acid> script;

    public QRunnerConfigurator(QAcidScript<Acid> script)
    {
        this.script = script;
    }

    public QRunner With(RunCount runCount) => new(script, runCount);
    public QRunner WithOneRun() => With(new(1));
}

public record RunCount(int NumberOfRuns);
public record ExecutionCount(int NumberOfExecutions);

public static class RunnerExtensions
{
    public static RunCount Runs(this int numberOfRuns) => new(numberOfRuns);
    public static ExecutionCount ExecutionsPerRun(this int numberOfExecutions) => new(numberOfExecutions);
}
