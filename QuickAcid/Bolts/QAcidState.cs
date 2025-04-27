using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.CodeGen;
using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class QAcidState : QAcidContext
{
    public QAcidRunner<Acid> Runner { get; private set; }
    public int CurrentExecutionNumber { get; private set; }
    public List<int> ExecutionNumbers { get; private set; }

    //only for report
    public int OriginalFailingRunExecutionCount { get; private set; }

    public Memory Memory { get; private set; }
    public ShrinkableInputsTracker ShrinkableInputsTracker { get; private set; }

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

    private int shrinkCount = 0;

    public string? FailingSpec { get { return CurrentContext.FailingSpec; } }
    public Exception? Exception { get { return CurrentContext.Exception; } }


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
        CheckAssays();
    }

    private void CheckAssays()
    {
        foreach (var (_, assay) in FinalChecks)
        {
            if (!assay.Check())
            {
                report.AddEntry(new ReportTitleSectionEntry([$"The Assayer disagrees : {assay.Label}."]));
                CurrentContext.Failed = true;
                break;
            }
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
                report.AddEntry(new ReportTitleSectionEntry(["FIRST FAILED RUN"]));
                report.AddEntry(new ReportRunStartEntry());
                AddMemoryToReport(report, false);
            }
            HandleFailure();
            return;
        }
        CurrentExecutionNumber++;
    }

    private void HandleFailure()
    {
        OriginalFailingRunExecutionCount = ExecutionNumbers.Count;
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
            title.AddRange(GetReportHeaderInfo().ToList());
            report.AddEntry(new ReportTitleSectionEntry(title));
            report.AddEntry(new ReportRunStartEntry());
        }
        else
        {
            report.AddEntry(new ReportTitleSectionEntry(GetReportHeaderInfo().ToList()));
            report.AddEntry(new ReportRunStartEntry());
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

    public readonly Dictionary<Type, IShrinkStrategy> ShrinkingStrategies
            = new Dictionary<Type, IShrinkStrategy>();

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
                    Memory.ResetAlwaysReportedMemory(); // NOT HAPPY ABOUT THIS, Strike needs this somehow
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

    public record AssaySpecEntry(string Label, Func<bool> Check);
    public Dictionary<string, AssaySpecEntry> FinalChecks = new();

    public void AddAssay(string Label, Func<bool> Check)
    {
        FinalChecks[Label] = new AssaySpecEntry(Label, Check);
    }
}
