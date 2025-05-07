using QuickAcid.Bolts;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Reporting;

namespace QuickAcid;

public class QDiagnosticState
{
    private readonly QAcidState state;
    private Report? report;

    public QDiagnosticState(QAcidRunner<Acid> runner)
    {
        state = new QAcidState(runner);
    }

    public QDiagnosticState AllowShrinking(bool allowShrinking)
    {
        state.AllowShrinking = allowShrinking;
        return this;
    }

    public QDiagnosticState ObserveOnce()
    {
        Observe(1);
        return this;
    }

    public QDiagnosticState Observe(int executionsPerScope)
    {
        report = state.Observe(executionsPerScope);
        return this;
    }

    public Report? GetReport()
    {
        return report;
    }

    public record RunInformation(
        Memory memory,
        List<int> executionNumbers,
        string? failingSpec = null,
        Exception? exception = null
    );

    public RunInformation GetRunInformation()
    {
        return new(
            state.Memory.DeepCopy()
            , state.ExecutionNumbers
            , state.FailingSpec
            , state.Exception);
    }
    public QDiagnosticState WithFeedback()
    {
        state.AllowFeedbackShrinking = true;
        return this;
    }
    public QDiagnosticState Shrink(RunInformation runInformation)
    {
        var (memory, executionNumbers, failingSpec, exception) = runInformation;
        state.SetMemory(memory, executionNumbers);
        state.CurrentContext.FailingSpec = failingSpec;
        state.CurrentContext.Exception = exception;
        state.HandleFailure();
        report = state.GetReport();
        return this;
    }
}




