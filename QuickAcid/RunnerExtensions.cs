using QuickAcid.RunningThings;

namespace QuickAcid;

public static class RunnerExtensions
{
    public static RunCount Runs(this int numberOfRuns) => new(numberOfRuns);
    public static ExecutionCount ExecutionsPerRun(this int numberOfExecutions) => new(numberOfExecutions);
}
