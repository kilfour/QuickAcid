using QuickAcid.CodeGen;
using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class QAcidState : QAcidContext
{
    public QAcidRunner<Acid> Runner { get; private set; }
    public int CurrentExecutionNumber { get; private set; }
    public List<int> ExecutionNumbers { get; private set; }

    public Memory Memory { get; private set; }
    public ShrinkableInputsTracker ShrinkableInputsTracker { get; private set; }
    public AlwaysReportedInputMemory AlwaysReportedInputsMemory { get; private set; }

    // ---------------------------------------------------------------------------------------
    // -- PHASERS
    private readonly Dictionary<QAcidPhase, PhaseContext> phaseContexts =
        Enum.GetValues<QAcidPhase>().ToDictionary(phase => phase, _ => new PhaseContext());
    public QAcidPhase CurrentPhase { get; private set; } = QAcidPhase.NormalRun;
    public PhaseContext CurrentContext => phaseContexts[CurrentPhase];

    public bool IsNormalRun => CurrentPhase == QAcidPhase.NormalRun;
    public bool IsShrinkingInputs => CurrentPhase == QAcidPhase.ShrinkingInputs;
    public bool IsShrinkInputEval => CurrentPhase == QAcidPhase.ShrinkInputEval;
    public bool IsShrinkingExecutions => CurrentPhase == QAcidPhase.ShrinkingExecutions;

    public IDisposable EnterPhase(QAcidPhase phase)
    {
        var previousPhase = CurrentPhase;
        CurrentPhase = phase;
        CurrentContext.Reset();
        return new DisposableAction(() => { CurrentPhase = previousPhase; });
    }
    // ---------------------------------------------------------------------------------------

    private int shrinkCount = 0;

    public string? FailingSpec { get; private set; }
    public Exception? Exception { get; private set; }


    private readonly QAcidReport report;
    public bool Verbose { get; set; }


    // ----------------------------------------------------------------------------------
    // only used by codegen, also usefull for tracking combinators with QAcidDebug 
    public XMarksTheSpot XMarksTheSpot { get; set; }
    public void MarkMyLocation(Tracker tracker)
    {
        XMarksTheSpot.TheTracker = tracker;
    }
    // ----------------------------------------------------------------------------------

    public QAcidState(QAcidRunner<Acid> runner)
    {
        Runner = runner;
        ExecutionNumbers = [];
        Memory = new Memory(() => CurrentExecutionNumber);
        ShrinkableInputsTracker = new ShrinkableInputsTracker(() => CurrentExecutionNumber);
        AlwaysReportedInputsMemory = new AlwaysReportedInputMemory(() => CurrentExecutionNumber);
        XMarksTheSpot = new XMarksTheSpot();
        report = new QAcidReport();
    }

    // -----------------------------------------------------------------
    // implementing context
    // --
    public T GetItAtYourOwnRisk<T>(string key) => Memory.GetForFluentInterface<T>(key);
    public T Get<T>(QKey<T> key) => GetItAtYourOwnRisk<T>(key.Label);
    // -----------------------------------------------------------------

    public void Run(int executionsPerScope)
    {
        for (int j = 0; j < executionsPerScope; j++)
        {
            ExecuteStep();
            if (CurrentContext.Failed)
                return;
        }
    }

    private void ExecuteStep()
    {
        ExecutionNumbers.Add(CurrentExecutionNumber);
        Runner(this);
        if (CurrentContext.Failed)
        {
            if (Verbose)
            {
                report.AddEntry(new ReportTitleSectionEntry("FIRST FAILED RUN"));
                AddMemoryToReport(report);
            }
            HandleFailure();
            return;
        }
        CurrentExecutionNumber++;
    }

    public void SpecFailed(string failingSpec)
    {
        CurrentContext.Failed = true;
        FailingSpec = failingSpec;
    }

    public void FailedWithException(Exception exception)
    {
        if (IsShrinkingExecutions)
        {
            if (Exception == null)
            {
                CurrentContext.BreakRun = true;
                CurrentContext.Failed = true;
                return;
            }

            if (Exception.GetType() != exception.GetType())
            {
                CurrentContext.BreakRun = true;
                CurrentContext.Failed = true;
                return;
            }
        }
        CurrentContext.Failed = true;
        Exception = exception;
    }

    private void HandleFailure()
    {
        ShrinkExecutions();
        if (Verbose)
        {
            report.AddEntry(new ReportTitleSectionEntry("AFTER EXECUTION SHRINKING"));
            AddMemoryToReport(report);
        }
        ShrinkInputs();
        if (Verbose)
        {
            report.AddEntry(new ReportTitleSectionEntry($"AFTER INPUT SHRINKING : Falsified after {ExecutionNumbers.Count} actions, {shrinkCount} shrinks"));
        }
        else
        {
            report.AddEntry(new ReportTitleSectionEntry($"Falsified after {ExecutionNumbers.Count} actions, {shrinkCount} shrinks"));
        }

        AddMemoryToReport(report);
    }

    private void ShrinkExecutions()
    {
        using (EnterPhase(QAcidPhase.ShrinkingExecutions))
        {
            var failingSpec = FailingSpec;
            var exception = Exception;
            var max = ExecutionNumbers.Max();
            var current = 0;
            while (current <= max)
            {
                CurrentContext.Failed = false;
                Memory.ResetAllRunInputs();
                FailingSpec = failingSpec;
                Exception = exception;
                foreach (var run in ExecutionNumbers.ToList())
                {
                    CurrentExecutionNumber = run;
                    if (run != current)
                        Runner(this);
                    if (CurrentContext.BreakRun)
                        break;
                }
                if (CurrentContext.Failed && !CurrentContext.BreakRun)
                {
                    ExecutionNumbers.Remove(current);
                }
                current++;
                shrinkCount++;
            }
            CurrentContext.Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;
        }
    }

    private void ShrinkInputs()
    {
        using (EnterPhase(QAcidPhase.ShrinkingInputs))
        {
            var failingSpec = FailingSpec;
            var exception = Exception;
            Memory.ResetAllRunInputs();
            foreach (var executionNumber in ExecutionNumbers.ToList())
            {
                CurrentExecutionNumber = executionNumber;
                Runner(this);
                shrinkCount++;
            }
            CurrentContext.Failed = true;
            FailingSpec = failingSpec;
            Exception = exception;
        }
    }

    public bool ShrinkRun(object key, object value) // Only Used by Shrink.cs
    {
        using (EnterPhase(QAcidPhase.ShrinkInputEval))
        {
            Memory.ResetAllRunInputs();
            var failingSpec = FailingSpec;
            var exception = Exception;
            var runNumber = CurrentExecutionNumber;
            var oldVal = Memory.ForThisExecution().Get<object>(key);
            Memory.ForThisExecution().Set(key, value);

            foreach (var actionNumber in ExecutionNumbers)
            {
                CurrentExecutionNumber = actionNumber;
                Runner(this);
            }
            var failed = CurrentContext.Failed;
            CurrentExecutionNumber = runNumber;
            CurrentContext.Failed = false;
            FailingSpec = failingSpec;
            Exception = exception;
            // USES CURRENT EXECUTION NUMBER (see above)
            Memory.ForThisExecution().Set(key, oldVal);
            return failed;
        }
    }

    private QAcidReport AddMemoryToReport(QAcidReport report)
    {
        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            MemoryReportAssembler.AddAllMemoryToReport(report, Memory, executionNumber, Exception!);
        }
        if (!string.IsNullOrEmpty(FailingSpec))
            report.AddEntry(new ReportSpecEntry(FailingSpec));
        return report;
    }

    public QAcidReport GetReport()
    {
        return report;
    }

    public void ThrowFalsifiableExceptionIfFailed()
    {
        if (CurrentContext.Failed)
        {
            throw new FalsifiableException(report.ToString(), Exception!)
            {
                QAcidReport = report,
            };
        }
    }

    public ExecutionContext GetExecutionContext()
    {
        return new ExecutionContext(Memory.ForThisExecution(), ShrinkableInputsTracker.ForThisExecution());
    }
}
