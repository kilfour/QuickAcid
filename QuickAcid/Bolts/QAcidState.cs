using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Phasers;
using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickAcid.Shrinking;
using QuickAcid.Shrinking.Runners;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;


namespace QuickAcid.Bolts;

public sealed class QAcidState
{
    public State FuzzState { get; } = new State();
    public int Seed { get { return FuzzState.Seed; } }

    public Verifier VerifyIf { get; }

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

    //--
    public ShrinkingRegistry ShrinkingRegistry { get; } = new ShrinkingRegistry();
    public Shifter Shifter { get; } = new Shifter();

    // only for report
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

    public void RecordFailure(Exception ex)
    {
        Shifter.CurrentContext.MarkFailure(ex, Shifter.OriginalRun);
    }

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
        if (Shifter.CurrentPhase != QAcidPhase.NormalRun)
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
            if (Shifter.CurrentContext.Failed)
            {
                return caseFile;
            }
        }
        return CaseFile.Empty();
    }

    private CaseFile ExecuteStep()
    {
        ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
        Script(this);
        if (Shifter.CurrentContext.Failed)
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
                AssayerSpec: forAssayer ? Shifter.CurrentContext.FailingSpec : null,
                FailingSpec: forAssayer ? null : Shifter.CurrentContext.FailingSpec,
                Exception: Shifter.CurrentContext.Exception,
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
