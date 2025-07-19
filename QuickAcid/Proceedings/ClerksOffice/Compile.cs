using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Proceedings.ClerksOffice;

public static class Compile
{
    public static CaseFile TheCaseFile(Memory Memory, Dossier dossier)
    {
        var verdict = Verdict.FromDossier(dossier);
        foreach (var executionNumber in dossier.ExecutionNumbers.ToList())
        {
            verdict.AddExecutionDeposition(GetExecutionDeposition(Memory, executionNumber));
        }
        return new CaseFile().WithVerdict(verdict);
    }

    private static ExecutionDeposition GetExecutionDeposition(Memory Memory, int executionNumber)
    {
        var executionDeposition = new ExecutionDeposition(executionNumber);
        GetTrackedDepositions(Memory, executionNumber, executionDeposition);
        var access = Memory.For(executionNumber);
        GetActionDepositions(executionDeposition, access);
        GetInputDepositions(executionDeposition, access);
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

    private static void GetInputDepositions(ExecutionDeposition executionDeposition, Access access)
    {
        foreach (var (key, val) in access.GetAll())
        {
            var shrinkOutcome = val.GetShrinkOutcome();
            if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            {
                executionDeposition.AddInputDeposition(new InputDeposition(key, msg));
            }
        }
    }

    private static void GetActionDepositions(ExecutionDeposition executionDeposition, Access access)
    {
        foreach (var action in access.ActionKeys)
        {
            executionDeposition.AddActionDeposition(new ActionDeposition(action));
        }
    }
}


// else if (!isFinalRun)
// {
//     if (val.ReportingIntent != ReportingIntent.Never)
//         executionDeposition.AddInputDeposition(new InputDeposition(key, val.Value!));
// }