using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Shrinking;
using QuickAcid.TheyCanFade;
using QuickPulse.Show;

namespace QuickAcid.Proceedings.ClerksOffice;

public static class Compile
{
    public static CaseFile TheCaseFile(Memory Memory, Dossier dossier)
    {
        var verdict = Verdict.FromDossier(dossier);
        foreach (var executionNumber in dossier.ExecutionNumbers.ToList())
        {
            verdict.AddExecutionDeposition(GetExecutionDeposition(Memory, executionNumber, true));
        }
        return CaseFile.WithVerdict(verdict);
    }

    public static RunDeposition TheRun(string label, Memory memory, List<int> ExecutionNumbers)
    {
        var runDeposition = new RunDeposition(label);
        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            runDeposition.AddExecutionDeposition(GetExecutionDeposition(memory, executionNumber, false));
        }
        return runDeposition;
    }

    private static ExecutionDeposition GetExecutionDeposition(Memory memory, int executionNumber, bool isVerdict)
    {
        var executionDeposition = new ExecutionDeposition(executionNumber);
        GetTrackedDepositions(memory, executionNumber, executionDeposition);
        var access = memory.For(executionNumber);
        GetActionDepositions(executionDeposition, access);
        GetInputDepositions(executionDeposition, access, isVerdict);
        GetTraceDepositions(executionDeposition, memory.TracesFor(executionNumber));
        return executionDeposition;
    }

    private static void GetTrackedDepositions(Memory Memory, int executionNumber, ExecutionDeposition executionDeposition)
    {
        if (Memory.TrackedSnapshot().TryGetValue(executionNumber, out var snapshot))
        {
            foreach (var (key, val) in snapshot)
                executionDeposition.AddTrackedDeposition(new TrackedDeposition(key, val));
        }
    }

    private static void GetActionDepositions(ExecutionDeposition executionDeposition, Access access)
    {
        foreach (var action in access.ActionKeys)
        {
            executionDeposition.AddActionDeposition(new ActionDeposition(action));
        }
    }

    private static void GetInputDepositions(ExecutionDeposition executionDeposition, Access access, bool isVerdict)
    {
        foreach (var (key, val) in access.GetAll())
        {
            var shrinkOutcome = val.GetShrinkOutcome();
            if (!isVerdict)
            {
                if (val.ReportingIntent != ReportingIntent.Never)
                    executionDeposition.AddInputDeposition(new InputDeposition(key, Introduce.This(val.Value!, false)));
            }
            else if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            {
                executionDeposition.AddInputDeposition(new InputDeposition(key, msg));
            }
        }
    }

    private static void GetTraceDepositions(ExecutionDeposition executionDeposition, Dictionary<string, string> traces)
    {
        foreach (var (key, val) in traces)
        {
            executionDeposition.AddTraceDeposition(new TraceDeposition(key, val));
        }
    }
}