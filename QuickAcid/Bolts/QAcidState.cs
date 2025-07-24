using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickAcid.ShrinkRunners;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;


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
    public Verifier VerifyIf { get; } // initialized in ctor with (this)

    public bool AllowShrinking = true; // toggle for Assayer et al
    private int shrinkCount = 0;
    // ---------------------------------------------------------------------------------------

    // ---------------------------------------------------------------------------------------
    // Configurable options
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

    public CaseFile Run(int executionsPerScope)
    {
        ExecutionNumbers = [.. Enumerable.Repeat(-1, executionsPerScope)];
        for (int j = 0; j < executionsPerScope; j++)
        {
            var caseFile = ExecuteStep();
            if (CurrentContext.Failed)
            {
                return caseFile;
            }
        }
        // foreach (var executionNumber in ExecutionNumbers.ToList())
        // {
        //     foreach (var (key, val) in Memory.TracesFor(executionNumber))
        //     {
        //         report.AddEntry(new ReportTraceEntry(key) { Value = val });
        //     }
        // }
        return CaseFile.Empty();
    }

    private CaseFile ExecuteStep()
    {
        ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
        Script(this);
        if (CurrentContext.Failed)
        {
            return HandleFailure();
        }
        CurrentExecutionNumber++;
        return CaseFile.Empty();
    }

    public CaseFile HandleFailure()
    {
        var runs = new List<RunDeposition>();
        if (Verbose)
        {
            runs.Add(DeposeTheRun("FIRST FAILED RUN"));
        }
        ExecutionNumbers = [.. ExecutionNumbers.Where(x => x != -1)];
        OriginalFailingRunExecutionCount = ExecutionNumbers.Count;
        if (AllowShrinking)
        {
            shrinkCount += ExecutionShrinker.Run(this);
            if (Verbose)
            {
                runs.Add(DeposeTheRun("AFTER EXECUTION SHRINKING"));
            }
            if (ShrinkingActions)
            {
                shrinkCount += ActionShrinker.Run(this);
                if (Verbose)
                {
                    runs.Add(DeposeTheRun("AFTER ACTION SHRINKING"));
                }
            }
            shrinkCount += InputShrinker.Run(this);
        }
        var forAssayer = !AllowShrinking;
        var caseFile = CompileTheCaseFile(forAssayer);
        foreach (var run in runs)
        {
            caseFile.AddRunDeposition(run);
        }
        return caseFile;
        // report.ShrinkTraces =
        //     Memory.AllAccesses()
        //         .SelectMany(a => a.access.GetAll().SelectMany(kv => kv.Value.GetShrinkTraces()))
        //         .ToList();
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

    private RunDeposition DeposeTheRun(string label)
    {
        return Compile.TheRun(label, Memory, ExecutionNumbers);
    }
}
