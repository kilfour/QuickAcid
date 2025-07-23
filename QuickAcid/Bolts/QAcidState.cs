using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickAcid.Reporting;
using QuickAcid.ShrinkRunners;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using QuickPulse;


namespace QuickAcid.Bolts;

public sealed class QAcidState
{
    public State FuzzState { get; } = new State();
    public int Seed { get { return FuzzState.Seed; } }

    public QAcidState(QAcidScript<Acid> script)
    {
        Script = script;
        ExecutionNumbers = [];
        Memory = new Memory(() => CurrentExecutionNumber);
        InputTracker = new InputTracker(() => CurrentExecutionNumber);
        report = new Report();
        VerifyIf = new Verifier(this);
    }

    public QAcidState(QAcidScript<Acid> script, int seed)
        : this(script)
    {
        FuzzState = new State(seed);
    }

    public QAcidScript<Acid> Script { get; private set; }

    public int CurrentExecutionNumber { get; set; }
    public List<int> ExecutionNumbers { get; private set; }

    public bool IsThisTheRunsLastExecution()
    {
        return CurrentExecutionNumber == ExecutionNumbers.Last();
    }

    //only for report
    public int OriginalFailingRunExecutionCount { get; private set; }

    public Memory Memory { get; private set; }
    public InputTracker InputTracker { get; private set; }
    public void SetMemory(Memory memory, List<int> executionNumbers)
    {
        Memory = memory;
        ExecutionNumbers = executionNumbers;
        Memory.SetCurrentActionIdFunction(() => CurrentExecutionNumber);
    }

    public RunExecutionContext CurrentExecutionContext()
    {
        return new RunExecutionContext(
            CurrentExecutionNumber,
            Memory.ForThisExecution(),
            InputTracker.ForThisExecution(),
            Memory.TracesForThisExecution());
    }

    public void Trace<T>(string key, string trace)
    {
        var execution = CurrentExecutionContext();
        execution.Trace(key, trace);
    }

    public void RecordFailure(Exception ex)
    {
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
        return new DisposableAction(() =>
        {
            CurrentPhase = previousPhase;
        });
    }

    public PhaseContext OriginalRun => Phase(QAcidPhase.NormalRun);
    // ---------------------------------------------------------------------------------------

    public ShrinkingRegistry ShrinkingRegistry { get; } = new ShrinkingRegistry();
    public Verifier VerifyIf { get; }

    public bool AllowShrinking = true;
    private int shrinkCount = 0;
    // ---------------------------------------------------------------------------------------
    private readonly Report report;
    public bool Verbose { get; set; }
    public bool ShrinkingActions { get; set; }


    // -----------------------------------------------------------------
    // spec counting
    // --
    private Dictionary<string, int> passedSpecCount = [];

    public void SpecPassed(string label)
    {
        if (CurrentPhase != QAcidPhase.NormalRun)
            return;
        if (!passedSpecCount.TryGetValue(label, out int value))
        {
            value = 0;
            passedSpecCount[label] = value;
        }
        passedSpecCount[label] = value + 1;
    }

    public void GetPassedSpecCount(Dictionary<string, int> collector)
    {
        foreach (var item in passedSpecCount)
        {
            if (collector.TryGetValue(item.Key, out int value))
                collector[item.Key] = value + item.Value;
            else
                collector[item.Key] = item.Value;

        }
    }

    public Report Run(int executionsPerScope)
    {
        ExecutionNumbers = [.. Enumerable.Repeat(-1, executionsPerScope)];
        for (int j = 0; j < executionsPerScope; j++)
        {
            ExecuteStep();
            if (CurrentContext.Failed)
            {
                return report;
            }
        }
        report.IsSuccess = true;
        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            foreach (var (key, val) in Memory.TracesFor(executionNumber))
            {
                report.AddEntry(new ReportTraceEntry(key) { Value = val });
            }
        }
        report.CaseFile = CaseFile.Empty();
        return report;
    }

    private void ExecuteStep()
    {
        ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
        Script(this);
        if (CurrentContext.Failed)
        {
            HandleFailure();
            return;
        }
        CurrentExecutionNumber++;
    }

    public void HandleFailure()
    {
        var forAssayer = false;
        var runs = new List<RunDeposition>();
        if (Verbose)
        {
            report.AddEntry(new ReportTitleSectionEntry(["FIRST FAILED RUN"]));
            AddMemoryToReport(report, false);
            runs.Add(WitnessTheRun("FIRST FAILED RUN"));

        }
        ExecutionNumbers = [.. ExecutionNumbers.Where(x => x != -1)];
        OriginalFailingRunExecutionCount = ExecutionNumbers.Count;
        if (AllowShrinking)
        {
            shrinkCount += ExecutionShrinker.Run(this);
            if (Verbose)
            {
                report.AddEntry(new ReportTitleSectionEntry(["AFTER EXECUTION SHRINKING"]));
                AddMemoryToReport(report, false);
                runs.Add(WitnessTheRun("AFTER EXECUTION SHRINKING"));
            }
            if (ShrinkingActions)
            {
                shrinkCount += ActionShrinker.Run(this);
                if (Verbose)
                {
                    report.AddEntry(new ReportTitleSectionEntry(["AFTER ACTION SHRINKING"]));
                    AddMemoryToReport(report, false);
                    runs.Add(WitnessTheRun("AFTER ACTION SHRINKING"));
                }
            }
            shrinkCount += InputShrinker.Run(this);
            if (Verbose)
            {
                var title = new List<string>(["AFTER INPUT SHRINKING :"]);
                title.AddRange([.. GetReportHeaderInfo()]);
                report.AddEntry(new ReportTitleSectionEntry(title));
            }
            else
            {
                report.AddEntry(new ReportTitleSectionEntry([.. GetReportHeaderInfo()]));
            }
        }
        else
        {
            report.AddEntry(new ReportTitleSectionEntry([$"The Assayer disagrees: {CurrentContext.FailingSpec}."]));
            forAssayer = true;
        }
        AddMemoryToReport(report, true);
        report.CaseFile = CompileTheCaseFile(forAssayer);
        foreach (var run in runs)
        {
            report.CaseFile.AddRunDeposition(run);
        }
        report.ShrinkTraces =
            Memory.AllAccesses()
                .SelectMany(a => a.access.GetAll().SelectMany(kv => kv.Value.GetShrinkTraces()))
                .ToList();
    }

    private IEnumerable<string> GetReportHeaderInfo()
    {
        var executionsText = ExecutionNumbers.Count == 1 ? "execution" : "executions";
        var shrinkText = shrinkCount == 1 ? "shrink" : "shrinks";
        yield return $"Original failing run:    {OriginalFailingRunExecutionCount} {executionsText}";
        yield return $"Minimal failing case:    {ExecutionNumbers.Count} {executionsText} (after {shrinkCount} {shrinkText})";
        yield return $"Seed:                    {FuzzState.Seed}";
        yield break;
    }

    private CaseFile CompileTheCaseFile(bool forAssayer = false)
    {
        var dossier =
            new Dossier(
                AssayerSpec: forAssayer ? CurrentContext.FailingSpec : null,
                FailingSpec: forAssayer ? null : CurrentContext.FailingSpec,
                Exception: CurrentContext.Exception,
                OriginalRunExecutionCount: OriginalFailingRunExecutionCount,
                ExecutionNumbers: ExecutionNumbers,
                ShrinkCount: shrinkCount,
                Seed: FuzzState.Seed
            );
        return Compile.TheCaseFile(Memory, dossier);
    }

    private RunDeposition WitnessTheRun(string label)
    {
        return Compile.TheRun(label, Memory, ExecutionNumbers);
    }

    private Report AddMemoryToReport(Report report, bool isFinalRun)
    {
        ReportExecutionEntry? collapsed = null;
        var collapsedCount = 0;

        void FlushCollapsed()
        {
            if (collapsedCount == 1)
                report.Entries.Add(collapsed!);
            else if (collapsed != null)
                report.Entries.Add(new ReportCollapsedExecutionEntry(collapsed.Key, collapsedCount));

            collapsed = null;
            collapsedCount = 0;
        }

        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            var entries = MemoryReportAssembler.GetReportEntriesForExecution(Memory, executionNumber, isFinalRun);

            if (entries.Count == 1 && entries[0] is ReportExecutionEntry entry)
            {
                if (collapsed == null)
                {
                    collapsed = entry;
                    collapsedCount = 1;
                }
                else if (collapsed.Key == entry.Key)
                {
                    collapsedCount++;
                }
                else
                {
                    FlushCollapsed();
                    collapsed = entry;
                    collapsedCount = 1;
                }
            }
            else
            {
                FlushCollapsed();
                report.Entries.AddRange(entries);
            }
        }

        FlushCollapsed();

        if (!string.IsNullOrEmpty(CurrentContext.FailingSpec))
            report.AddEntry(new ReportSpecEntry(LabelPrettifier.Prettify(CurrentContext.FailingSpec)));
        if (CurrentContext.Exception != null)
        {
            report.Exception = CurrentContext.Exception; // TODO remove this
            report.AddEntry(new ReportExceptionEntry(CurrentContext.Exception));
        }
        return report;
    }

    public Report GetReport()
    {
        return report;
    }
}
