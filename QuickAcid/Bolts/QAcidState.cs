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

    public QAcidPhase CurrentPhase { get; set; } = QAcidPhase.NormalRun;

    public bool IsNormalRun => CurrentPhase == QAcidPhase.NormalRun;
    public bool IsShrinkingInputs => CurrentPhase == QAcidPhase.ShrinkingInputs;
    public bool IsShrinkInputEval => CurrentPhase == QAcidPhase.ShrinkInputEval;
    public bool IsShrinkingExecutions => CurrentPhase == QAcidPhase.ShrinkingExecutions;

    private int shrinkCount = 0;

    public bool Failed { get; private set; }
    public bool BreakRun { get; private set; }

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
            ExecuteRun();
            if (Failed)
                return;
        }
    }

    private void ExecuteRun()
    {
        ExecutionNumbers.Add(CurrentExecutionNumber);
        Runner(this);
        if (Failed)
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

    public void FailedWithException(Exception exception)
    {
        if (CurrentPhase == QAcidPhase.ShrinkingExecutions)
        {
            if (Exception == null)
            {
                BreakRun = true;
                Failed = true;
                return;
            }

            if (Exception.GetType() != exception.GetType())
            {
                BreakRun = true;
                Failed = true;
                return;
            }
        }
        Failed = true;
        Exception = exception;
    }

    public void SpecFailed(string failingSpec)
    {
        Failed = true;
        FailingSpec = failingSpec;
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
        CurrentPhase = QAcidPhase.ShrinkingExecutions;
        BreakRun = false;

        Failed = false;
        var failingSpec = FailingSpec;
        var exception = Exception;

        var max = ExecutionNumbers.Max();
        var current = 0;

        while (current <= max)
        {
            Failed = false;
            Memory.ResetAllRunInputs();
            FailingSpec = failingSpec;
            Exception = exception;

            foreach (var run in ExecutionNumbers.ToList())
            {
                CurrentExecutionNumber = run;
                if (run != current)
                    Runner(this);
                if (BreakRun)
                    break;
            }
            if (Failed && !BreakRun)
            {
                ExecutionNumbers.Remove(current);
            }
            current++;
            shrinkCount++;
        }

        Failed = true;
        FailingSpec = failingSpec;
        Exception = exception;
    }

    private void ShrinkInputs()
    {
        CurrentPhase = QAcidPhase.ShrinkingInputs;
        Failed = false;
        var failingSpec = FailingSpec;
        var exception = Exception;
        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            Memory.ResetAllRunInputs();
            CurrentExecutionNumber = executionNumber;
            Runner(this);
            shrinkCount++;
        }
        Failed = true;
        FailingSpec = failingSpec;
        Exception = exception;
    }

    public bool ShrinkRun(object key, object value) // Only Used by Shrink.cs
    {
        var oldPhase = CurrentPhase;
        CurrentPhase = QAcidPhase.ShrinkInputEval;
        Failed = false;
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
        var failed = Failed;
        CurrentExecutionNumber = runNumber;
        Failed = false;
        FailingSpec = failingSpec;
        Exception = exception;
        // USES CURRENT EXECUTION NUMBER (see above)
        Memory.ForThisExecution().Set(key, oldVal);
        CurrentPhase = oldPhase;
        return failed;
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
        if (Failed)
        {
            throw new FalsifiableException(report.ToString(), Exception!)
            {
                QAcidReport = report,
            };
        }
    }

    internal ExecutionContext GetExecutionContext()
    {
        return new ExecutionContext(Memory.ForThisExecution(), ShrinkableInputsTracker.ForThisExecution());
    }
}

public class ExecutionContext
{
    private readonly Access memory;
    private readonly ShrinkableInputsTrackerPerExecution shrinkTracker;

    public ExecutionContext(Access memory, ShrinkableInputsTrackerPerExecution shrinkTracker)
    {
        this.memory = memory;
        this.shrinkTracker = shrinkTracker;
    }

    public bool AlreadyTried(string key) => shrinkTracker.AlreadyTried(key);

    public void SetShrinkOutcome(string key, ShrinkOutcome outcome)
    {
        shrinkTracker.MarkAsTriedToShrink(key);
        if (memory.ContainsKey(key))
        {
            memory.SetShrinkOutcome(key, outcome);
        }
    }

    public T Get<T>(string key) => memory.Get<T>(key);

    public Maybe<T> GetMaybe<T>(string key) => memory.GetMaybe<T>(key);

    public void SetIfAbsent<T>(string key, T value) => memory.SetIfNotAllReadyThere(key, value);
}

public enum QAcidPhase
{
    NormalRun,
    ShrinkingExecutions,
    ShrinkingInputs,
    ShrinkInputEval
}