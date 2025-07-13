using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.ShrinkStrats.Collections;
using QuickAcid.Bolts.ShrinkStrats.Objects;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Reporting;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;


namespace QuickAcid.Bolts;

public sealed class QAcidState : QAcidContext
{

    public State MGenState { get; } = new State();

    // TODO MAKE INTERNAL
    public QAcidState(QAcidScript<Acid> script)
    {
        Script = script;
        ExecutionNumbers = [];
        Memory = new Memory(() => CurrentExecutionNumber);
        InputTracker = new InputTracker(() => CurrentExecutionNumber);
        report = new Report();
    }

    public QAcidState(QAcidScript<Acid> script, int seed)
        : this(script)
    {
        MGenState = new State(seed);
    }

    // -------------------------------------------------------------------------------------------------
    // -- New Way of Shrinking
    // --
    public void Trace(string key, ShrinkKind shrinkKind, ShrinkTrace trace)
    {
        Memory.ForThisExecution().GetDecorated(key).AddTrace(shrinkKind, trace);
    }
    // -------------------------------------------------------------------------------------------------

    public QAcidScript<Acid> Script { get; private set; }
    public int CurrentExecutionNumber { get; set; }
    public IDisposable SetCurrentExecutionNumber(int number)
    {
        var previousNumber = CurrentExecutionNumber;
        CurrentExecutionNumber = number;
        return new DisposableAction(() => { CurrentExecutionNumber = previousNumber; });
    }

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
    public RunExecutionContext GetExecutionContext()
    {
        return new RunExecutionContext(
            Memory.ForThisExecution(),
            InputTracker.ForThisExecution(),
            Memory.TracesForThisExecution());
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

    public void Trace<T>(string key, string trace)
    {
        var execution = GetExecutionContext();
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
    public bool InFeedbackShrinkingPhase = false;
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


    // ----------------------------------------------------------------------------------
    // Shrinking Strategies
    private readonly Dictionary<Type, object> shrinkers = [];
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

    private readonly Dictionary<(Type, PropertyInfo), IShrinkerBox> propertyShrinkers = [];
    public void RegisterPropertyShrinker<T, TProp>(Expression<Func<T, TProp>> expr, IShrinker<TProp> shrinker)
    {
        var info = expr.AsPropertyInfo();
        propertyShrinkers[(typeof(T), info)] = new ShrinkerBox<TProp>(shrinker);
    }
    public IShrinkerBox? TryGetPropertyShrinker<T>(PropertyInfo info)
    {
        return propertyShrinkers.TryGetValue((typeof(T), info), out var shrinker)
            ? shrinker
            : null;
    }

    public Func<IEnumerable<ICollectionShrinkStrategy>> GetCollectionStrategies =
        () => [new RemoveOneByOneStrategy(), new ShrinkEachElementStrategy()];

    public Func<IEnumerable<IObjectShrinkStrategy>> GetObjectStrategies =
        () => [new ObjectShrinkStrategy()];
    // ---------------------------------------------------------------------------------------
    public bool AllowShrinking = true;
    public bool AllowFeedbackShrinking = false;
    private int shrinkCount = 0;
    // ---------------------------------------------------------------------------------------
    private readonly Report report;
    public bool Verbose { get; set; }
    public bool AlwaysReport { get; set; }
    public bool ShrinkingActions { get; set; }

    private Dictionary<string, int> passedSpecCount = [];
    public void SpecPassed(string label)
    {
        if (!passedSpecCount.ContainsKey(label))
        {
            passedSpecCount[label] = 0;
        }
        passedSpecCount[label] = passedSpecCount[label] + 1;
    }
    // -----------------------------------------------------------------
    // implementing context for fluent
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

    public Report ObserveOnce()
    {
        return Observe(1);
    }

    public Report Observe(int executionsPerScope)
    {
        return Run(executionsPerScope, false);
    }

    public Report Run(int executionsPerScope, bool throwOnFailure)
    {
        ExecutionNumbers = [.. Enumerable.Repeat(-1, executionsPerScope)];
        for (int j = 0; j < executionsPerScope; j++)
        {
            ExecuteStep();
            if (CurrentContext.Failed)
            {
                if (throwOnFailure)
                    throw new FalsifiableException(report, CurrentContext.Exception!);
                else
                    return report;
            }
            if (AlwaysReport)
            {
                AddMemoryToReport(report, true);
                return report;
            }
        }
        return null!;
    }

    private void ExecuteStep()
    {
        ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
        Script(this);
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
            if (ShrinkingActions)
            {
                ShrinkActions();
                if (Verbose)
                {
                    report.AddEntry(new ReportTitleSectionEntry(["AFTER ACTION SHRINKING"]));
                    report.AddEntry(new ReportRunStartEntry());
                    AddMemoryToReport(report, false);
                }
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
                if (AllowFeedbackShrinking)
                {
                    FeedbackShrinking();
                }
                report.AddEntry(new ReportTitleSectionEntry([.. GetReportHeaderInfo()]));
                report.AddEntry(new ReportRunStartEntry());
            }

        }
        else
        {
            report.AddEntry(new ReportTitleSectionEntry([$"The Assayer disagrees: {CurrentContext.FailingSpec}."]));
        }
        AddMemoryToReport(report, true);
        report.ShrinkTraces =
            Memory.AllAccesses()
                .SelectMany(a => a.access.GetAll().SelectMany(kv => kv.Value.GetShrinkTraces()))
                .ToList();
        report.PassedSpecCount = passedSpecCount.Select(kv => new SpecCount(Label: kv.Key, Count: kv.Value)).ToArray();
    }

    private IEnumerable<string> GetReportHeaderInfo()
    {
        if (!string.IsNullOrEmpty(CurrentContext.FailingSpec))
            yield return $"Property '{LabelPrettifier.Prettify(CurrentContext.FailingSpec)}' was falsified";
        if (CurrentContext.Exception != null)
            yield return "Exception thrown";
        yield return $"Original failing run: {OriginalFailingRunExecutionCount} execution(s)";
        yield return $"Shrunk to minimal case:  {ExecutionNumbers.Count} execution(s) ({shrinkCount} shrinks)";
        yield return $"Seed: {MGenState.Seed}";
        yield break;
    }

    public void ShrinkExecutions()
    {
        var max = ExecutionNumbers.Max();
        var current = 0;
        while (current <= max && ExecutionNumbers.Count() > 1)
        {
            using (EnterPhase(QAcidPhase.ShrinkingExecutions))
            {
                Memory.ResetRunScopedInputs();
                foreach (var executionNumber in ExecutionNumbers.ToList())
                {
                    CurrentExecutionNumber = executionNumber;
                    if (executionNumber != current)
                        Script(this);
                }
                if (CurrentContext.Failed)
                {
                    ExecutionNumbers.Remove(current);
                }
                current++;
                shrinkCount++;
            }
        }
    }

    public void ShrinkActions() // NEEDS CHANGING
    {
        var max = ExecutionNumbers.Max();
        foreach (var outerExcutionNumber in ExecutionNumbers.ToList())
        {
            var oldKeys = Memory.For(outerExcutionNumber).ActionKeys;
            if (oldKeys.Count < 1) continue;
            foreach (var key in oldKeys.ToList())
            {
                if (oldKeys.Count < 1) continue;
                Memory.For(outerExcutionNumber).ActionKeys.Remove(key);
                using (EnterPhase(QAcidPhase.ShrinkingExecutions))
                {
                    Memory.ResetRunScopedInputs();
                    foreach (var executionNumber in ExecutionNumbers.ToList())
                    {
                        CurrentExecutionNumber = executionNumber;
                        Script(this);
                    }
                    if (!CurrentContext.Failed)
                    {
                        Memory.For(outerExcutionNumber).ActionKeys.Add(key);
                    }
                    shrinkCount++;
                }
            }
        }
    }

    private void ShrinkInputs()
    {
        using (EnterPhase(QAcidPhase.ShrinkingInputs))
        {
            Memory.ResetRunScopedInputs();
            foreach (var executionNumber in ExecutionNumbers.ToList())
            {
                CurrentExecutionNumber = executionNumber;
                Script(this);
                shrinkCount++;
            }
        }
    }

    private void FeedbackShrinking()
    {
        InputTracker = new InputTracker(() => CurrentExecutionNumber);
        InFeedbackShrinkingPhase = true;
        var max = ExecutionNumbers.Max();
        var current = 0;
        while (current <= max && ExecutionNumbers.Count > 1)
        {
            using (EnterPhase(QAcidPhase.FeedbackShrinking))
            {
                Memory.ResetRunScopedInputs();
                foreach (var executionNumber in ExecutionNumbers.ToList())
                {
                    CurrentExecutionNumber = executionNumber;
                    if (executionNumber != current)
                        Script(this);
                }
                if (CurrentContext.Failed)
                {
                    ExecutionNumbers.Remove(current);
                }
                current++;
                shrinkCount++;
            }
        }
    }

    public bool ShrinkRunReturnTrueIfFailed(string key, object value)
    {
        using (EnterPhase(QAcidPhase.ShrinkInputEval))
        {
            CurrentContext.Reset();
            Memory.ResetRunScopedInputs();
            var runNumber = CurrentExecutionNumber;
            using (Memory.ScopedSwap(key, value))
            {
                foreach (var actionNumber in ExecutionNumbers)
                {
                    CurrentExecutionNumber = actionNumber;
                    Script(this);
                }
            }
            CurrentExecutionNumber = runNumber;
            return CurrentContext.Failed;
        }
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
            report.Exception = CurrentContext.Exception;
        return report;
    }

    public Report GetReport()
    {
        return report;
    }
}
