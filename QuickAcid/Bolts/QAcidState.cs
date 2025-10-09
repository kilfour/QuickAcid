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

    public Verifier VerifyIf { get; private set; }

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
    private int originalFailingRunExecutionCount;

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
            Memory.AccessForForThisExecution(),
            InputTracker.ForThisExecution(),
            Memory.TracesForThisExecution(),
            Memory.DiagnosisForThisExecution());
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
    // public bool Verbose { get; set; }
    // public bool ShrinkingActions { get; set; }

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

    public CaseFile Run(int executionsPerScope, QAcidStateConfig config)
    {

        ExecutionNumbers = [.. Enumerable.Repeat(-1, executionsPerScope)];
        for (int j = 0; j < executionsPerScope; j++)
        {
            var caseFile = PerformExecution(config);
            if (Shifter.CurrentContext.Failed)
            {
                return caseFile;
            }
        }
        //return Annotate.TheCaseFile(CaseFile.Empty(), Memory, ExecutionNumbers);
        return CaseFile.Empty();
    }
    // ---------------------------------------------------------------------
    // FOR SEEDVAULT
    public CaseFile Run(int executionsPerScope) => Run(executionsPerScope, new());
    // ---------------------------------------------------------------------

    private CaseFile PerformExecution(QAcidStateConfig config)
    {
        ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
        Script(this);
        if (Shifter.CurrentContext.Failed)
        {
            return HandleFailure(config);
        }
        CurrentExecutionNumber++;
        return CaseFile.Empty();
    }

    public CaseFile HandleFailure(QAcidStateConfig config)
    {
        var executionShrinker = config.Diagnose == null ? new ExecutionShrinker() : new DiagnosticExecutionShrinker(config.Diagnose);
        VerifyIf = config.Diagnose == null ? new Verifier(this) : new DiagnosticVerifier(this, config.Diagnose);
        var runs = new List<RunDeposition>();
        if (config.Verbose)
        {
            runs.Add(DeposeTheRun("FIRST FAILED RUN"));
        }
        ExecutionNumbers = [.. ExecutionNumbers.Where(x => x != -1)];
        originalFailingRunExecutionCount = ExecutionNumbers.Count;
        if (AllowShrinking)
        {
            shrinkCount += executionShrinker.Run(this);
            if (config.Verbose)
                runs.Add(DeposeTheRun("AFTER EXECUTION SHRINKING"));

            if (config.ShrinkingActions)
            {
                shrinkCount += ActionShrinker.Run(this);
                if (config.Verbose)
                    runs.Add(DeposeTheRun("AFTER ACTION SHRINKING"));
            }
            shrinkCount += InputShrinker.Run(this);
        }
        var forAssayer = !AllowShrinking;
        var caseFile = CompileTheCaseFile(forAssayer);

        foreach (var run in runs)
        {
            caseFile.AddRunDeposition(run);
        }
        caseFile.ShrinkTraces =
            Memory.AllAccesses()
                .SelectMany(a => a.access.GetAll().SelectMany(kv => kv.Value.GetShrinkTraces()))
                .ToList();
        return caseFile;

    }

    private CaseFile CompileTheCaseFile(bool forAssayer = false)
    {
        var dossier =
            new Dossier(
                AssayerSpec: forAssayer ? Shifter.CurrentContext.FailingSpec : null,
                FailingSpec: forAssayer ? null : Shifter.CurrentContext.FailingSpec,
                Exception: Shifter.CurrentContext.Exception,
                OriginalRunExecutionCount: originalFailingRunExecutionCount,
                ExecutionNumbers: ExecutionNumbers,
                ShrinkCount: shrinkCount,
                Seed: FuzzState.Seed
            );
        //return Annotate.TheCaseFile(Compile.TheCaseFile(Memory, dossier), Memory, dossier.ExecutionNumbers);
        return Compile.TheCaseFile(Memory, dossier);
    }

    private RunDeposition DeposeTheRun(string label)
    {
        return Compile.TheRun(label, Memory, ExecutionNumbers);
    }
}
