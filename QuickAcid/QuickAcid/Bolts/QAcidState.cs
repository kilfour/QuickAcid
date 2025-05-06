using QuickAcid.Bolts;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Reporting;

namespace QuickAcid;

public sealed class QAcidState : QAcidContext
{
    // TODO MAKE INTERNAL
    public QAcidState(QAcidRunner<Acid> runner)
    {
        Runner = runner;
        ExecutionNumbers = [];
        Memory = new Memory(() => CurrentExecutionNumber);
        ShrinkableInputsTracker = new ShrinkableInputsTracker(() => CurrentExecutionNumber);
        report = new QAcidReport();
    }

    public QAcidRunner<Acid> Runner { get; private set; }
    public int CurrentExecutionNumber { get; private set; }
    public List<int> ExecutionNumbers { get; private set; }
    public bool IsThisTheRunsLastExecution()
    {
        return CurrentExecutionNumber == ExecutionNumbers.Last();
    }

    //only for report
    public int OriginalFailingRunExecutionCount { get; private set; }

    public Memory Memory { get; private set; }
    public ShrinkableInputsTracker ShrinkableInputsTracker { get; private set; }
    public void SetMemory(Memory memory, List<int> executionNumbers)
    {
        Memory = memory;
        ExecutionNumbers = executionNumbers;
        Memory.SetCurrentActionIdFunction(() => CurrentExecutionNumber);
    }
    public RunExecutionContext GetExecutionContext()
    {
        return new RunExecutionContext(Memory.ForThisExecution(), ShrinkableInputsTracker.ForThisExecution());
    }

    public T Remember<T>(string key, Func<T> factory, ReportingIntent reportingIntent = ReportingIntent.Shrinkable)
    {
        var execution = GetExecutionContext();
        if (!execution.memory.ContainsKey(key))
        {
            var value = factory();
            execution.memory.Set(key, value, reportingIntent);
            return value;
        }
        return execution.Get<T>(key);
    }

    public void RecordFailure(Exception ex)
    {
        GetExecutionContext().AddException(ex);
        CurrentContext.MarkFailure(ex, OriginalRun);
    }

    // ---------------------------------------------------------------------------------------
    // -- PHASERS
    private readonly Dictionary<QAcidPhase, PhaseContext> phaseContexts =
        Enum.GetValues<QAcidPhase>().ToDictionary(phase => phase, phase => new PhaseContext(phase));
    public QAcidPhase CurrentPhase { get; private set; } = QAcidPhase.NormalRun;
    public PhaseContext CurrentContext => phaseContexts[CurrentPhase];
    public PhaseContext Phase(QAcidPhase phase) => phaseContexts[phase];

    public bool IsShrinkingExecutions => CurrentPhase == QAcidPhase.ShrinkingExecutions;

    public IDisposable EnterPhase(QAcidPhase phase)
    {
        var previousPhase = CurrentPhase;
        CurrentPhase = phase;
        CurrentContext.Reset();
        return new DisposableAction(() => { CurrentPhase = previousPhase; });
    }

    public PhaseContext OriginalRun => Phase(QAcidPhase.NormalRun);
    // ---------------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------
    // Shrinking Strategies
    private readonly Dictionary<Type, object> shrinkers = new();
    public void RegisterShrinker<T>(IShrinker<T> shrinker)
    {
        shrinkers[typeof(T)] = shrinker;
    }

    public IShrinker<T>? TryGetShrinker<T>()
    {
        return shrinkers.TryGetValue(typeof(T), out var shrinker)
            ? shrinker as IShrinker<T>
            : null;
    }
    // ---------------------------------------------------------------------------------------
    public bool AllowShrinking = true;
    private int shrinkCount = 0;
    public string? FailingSpec { get { return CurrentContext.FailingSpec; } }
    public Exception? Exception { get { return CurrentContext.Exception; } }


    private readonly QAcidReport report;
    public bool Verbose { get; set; }

    // -----------------------------------------------------------------
    // implementing context
    // --
    public T GetItAtYourOwnRisk<T>(string key) => Memory.GetForFluentInterface<T>(key);
    public T Get<T>(QKey<T> key) => GetItAtYourOwnRisk<T>(key.Label);
    // -----------------------------------------------------------------

    public void TestifyOnce()
    {
        Testify(1);
    }

    public void Testify(int executionsPerScope)
    {
        Run(executionsPerScope, true);
    }

    public QAcidReport ObserveOnce()
    {
        return Observe(1);
    }

    public QAcidReport Observe(int executionsPerScope)
    {
        return Run(executionsPerScope, false);
    }

    public QAcidReport Run(int executionsPerScope, bool throwOnFailure)
    {
        ExecutionNumbers = [.. Enumerable.Repeat(-1, executionsPerScope)];
        for (int j = 0; j < executionsPerScope; j++)
        {
            ExecuteStep();
            if (CurrentContext.Failed)
            {
                if (throwOnFailure)
                    throw new FalsifiableException(report.ToString(), Exception!) { QAcidReport = report };
                else
                    return report;
            }
            if (CurrentContext.BreakRun)
            {
                break;
            }
        }
        return null!;
    }

    private void ExecuteStep()
    {
        ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
        Runner(this);
        if (CurrentContext.Failed)
        {
            if (Verbose)
            {
                report.AddEntry(new ReportTitleSectionEntry(["FIRST FAILED RUN"]));
                report.AddEntry(new ReportRunStartEntry());
                AddMemoryToReport(report, false);
            }
            HandleFailure();
            return;
        }
        CurrentExecutionNumber++;
    }

    public void HandleFailure()
    {
        ExecutionNumbers = [.. ExecutionNumbers.Where(x => x != -1)];
        OriginalFailingRunExecutionCount = ExecutionNumbers.Count;
        if (AllowShrinking)
        {
            ShrinkExecutions();
            if (Verbose)
            {
                report.AddEntry(new ReportTitleSectionEntry(["AFTER EXECUTION SHRINKING"]));
                report.AddEntry(new ReportRunStartEntry());
                AddMemoryToReport(report, false);
            }
            ShrinkInputs();
            if (Verbose)
            {
                var title = new List<string>(["AFTER INPUT SHRINKING :"]);
                title.AddRange([.. GetReportHeaderInfo()]);
                report.AddEntry(new ReportTitleSectionEntry(title));
                report.AddEntry(new ReportRunStartEntry());
            }
            else
            {
                report.AddEntry(new ReportTitleSectionEntry([.. GetReportHeaderInfo()]));
                report.AddEntry(new ReportRunStartEntry());
            }
        }
        else
        {
            report.AddEntry(new ReportTitleSectionEntry([$"The Assayer disagrees: {FailingSpec}."]));
        }
        AddMemoryToReport(report, true);
        if (Exception != null)
            report.Exception = Exception;
    }

    private IEnumerable<string> GetReportHeaderInfo()
    {
        if (!string.IsNullOrEmpty(FailingSpec))
            yield return $"Property '{LabelPrettifier.Prettify(FailingSpec)}' was falsified";
        if (Exception != null)
            yield return "Exception thrown";
        yield return $"Original failing run: {OriginalFailingRunExecutionCount} execution(s)";
        yield return $"Shrunk to minimal case:  {ExecutionNumbers.Count} execution(s) ({shrinkCount} shrinks)";
        yield break;
    }

    private void ShrinkExecutions()
    {
        var max = ExecutionNumbers.Max();
        var current = 0;
        while (current <= max)
        {
            using (EnterPhase(QAcidPhase.ShrinkingExecutions))
            {
                Memory.ResetAllRunInputs();
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
        }
    }

    private void ShrinkInputs()
    {
        using (EnterPhase(QAcidPhase.ShrinkingInputs))
        {
            Memory.ResetAllRunInputs();
            foreach (var executionNumber in ExecutionNumbers.ToList())
            {
                CurrentExecutionNumber = executionNumber;
                Runner(this);
                shrinkCount++;
            }
        }
    }

    public bool ShrinkRun(object key, object value) // Only Used by Shrink.cs
    {
        using (EnterPhase(QAcidPhase.ShrinkInputEval))
        {
            Memory.ResetAllRunInputs();
            var runNumber = CurrentExecutionNumber;
            using (Memory.ScopedSwap(key, value))
            {
                foreach (var actionNumber in ExecutionNumbers)
                {
                    CurrentExecutionNumber = actionNumber;
                    Runner(this);
                }
            }
            CurrentExecutionNumber = runNumber;
            return CurrentContext.Failed;
        }
    }

    private QAcidReport AddMemoryToReport(QAcidReport report, bool isFinalRun)
    {
        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            MemoryReportAssembler
                .AddAllMemoryToReport(report, Memory, executionNumber, Exception!, isFinalRun);
        }
        if (!string.IsNullOrEmpty(FailingSpec))
            report.AddEntry(new ReportSpecEntry(LabelPrettifier.Prettify(FailingSpec)));
        return report;
    }

    public QAcidReport GetReport()
    {
        return report;
    }
}
