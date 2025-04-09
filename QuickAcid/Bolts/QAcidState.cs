using QuickAcid.CodeGen;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class QAcidState : QAcidContext
{
    public QAcidRunner<Acid> Runner { get; private set; }

    public int CurrentExcutionNumber { get; private set; }

    public List<int> ExcutionNumbers { get; private set; }

    public Memory Memory { get; private set; }


    public bool Shrinking { get; private set; }
    public bool ShrinkingExecutions { get; private set; }
    public Memory Shrunk { get; private set; }
    private int shrinkCount = 0;

    public bool Verifying { get; private set; } // NEEDS TO GO replace all flags with phase struct

    public bool Failed { get; private set; }
    public string? FailingSpec { get; private set; }
    public Exception? Exception { get; private set; }

    private readonly QAcidReport report;

    public bool Verbose { get; set; }


    // only used by codegen, also usefull for tracking runners with QAcidDebug 
    public XMarksTheSpot XMarksTheSpot { get; set; }
    public void MarkMyLocation(Tracker tracker)
    {
        XMarksTheSpot.TheTracker = tracker;
    }

    public QAcidState(QAcidRunner<Acid> runner)
    {
        Runner = runner;
        ExcutionNumbers = new List<int>();
        Memory = new Memory(() => CurrentExcutionNumber);
        Shrunk = new Memory(() => CurrentExcutionNumber);
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
            StepAction();
            if (Failed)
                return;
        }
    }

    private void StepAction()
    {
        ExcutionNumbers.Add(CurrentExcutionNumber);
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
        CurrentExcutionNumber++;
    }

    public bool IsNormalRun()
    {
        return Verifying == false && Shrinking == false && ShrinkingExecutions == false;
    }

    public void FailedWithException(Exception exception)
    {
        if (Verifying)
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
        ShrinkActions();
        if (Verbose)
        {
            report.AddEntry(new ReportTitleSectionEntry("AFTER EXECUTION SHRINKING"));
            AddMemoryToReport(report);
        }
        ShrinkInputs();
        if (Verbose)
        {
            report.AddEntry(new ReportTitleSectionEntry($"AFTER INPUT SHRINKING : Falsified after {ExcutionNumbers.Count} actions, {shrinkCount} shrinks"));
        }
        else
        {
            report.AddEntry(new ReportTitleSectionEntry($"Falsified after {ExcutionNumbers.Count} actions, {shrinkCount} shrinks"));
        }

        AddMemoryToReport(report);
    }

    public bool BreakRun { get; private set; }

    private void ShrinkActions()
    {
        Verifying = true;
        ShrinkingExecutions = true;
        Shrinking = false;
        BreakRun = false;

        Failed = false;
        var failingSpec = FailingSpec;
        var exception = Exception;

        var max = ExcutionNumbers.Max();
        var current = 0;

        while (current <= max)
        {
            Failed = false;
            Memory.ResetAllRunInputs();
            FailingSpec = failingSpec;
            Exception = exception;

            foreach (var run in ExcutionNumbers.ToList())
            {
                CurrentExcutionNumber = run;
                if (run != current)
                    Runner(this);
                if (BreakRun)
                    break;
            }
            if (Failed && !BreakRun)
            {
                ExcutionNumbers.Remove(current);
            }
            current++;
            shrinkCount++;
        }

        Failed = true;
        FailingSpec = failingSpec;
        Exception = exception;

        // Verifying = false;
        ShrinkingExecutions = false;
        // Shrinking = false;
        // BreakRun = false;
    }

    private void ShrinkInputs()
    {
        Verifying = false;
        Shrinking = true;

        Failed = false;


        var failingSpec = FailingSpec;
        var exception = Exception;

        foreach (var run in ExcutionNumbers.ToList())
        {
            Memory.ResetAllRunInputs();
            CurrentExcutionNumber = run;
            Runner(this);
            shrinkCount++;
        }

        Failed = true;
        FailingSpec = failingSpec;
        Exception = exception;
    }

    public bool ShrinkRun(object key, object value) // Only Used by Shrink.cs
    {
        Verifying = true;
        Shrinking = false;
        Failed = false;
        Memory.ResetAllRunInputs();
        var failingSpec = FailingSpec;
        var exception = Exception;
        var runNumber = CurrentExcutionNumber;
        var oldVal = Memory.ForThisAction().Get<object>(key);
        Memory.ForThisAction().Set(key, value);

        foreach (var actionNumber in ExcutionNumbers)
        {
            CurrentExcutionNumber = actionNumber;
            Runner(this);
        }
        var failed = Failed;
        CurrentExcutionNumber = runNumber;
        Failed = false;
        FailingSpec = failingSpec;
        Exception = exception;
        Verifying = false;
        Shrinking = true;
        Memory.ForThisAction().Set(key, oldVal);

        return failed;
    }

    private QAcidReport AddMemoryToReport(QAcidReport report)
    {
        foreach (var actionNumber in ExcutionNumbers.ToList())
        {
            Memory.AddActionToReport(actionNumber, report, Exception!);
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
                MemoryDump = Memory.ToDiagnosticString()
            };
        }
    }

    internal QAcidState Fail()
    {
        throw new NotImplementedException();
    }
}
