// namespace QuickAcid.Bolts;

// public abstract record ExecutionOutcome
// {
//     public static ExecutionOutcome SpecFailed(string specName) => new SpecFailedOutcome(specName);
//     public sealed record SpecFailedOutcome(string SpecName) : ExecutionOutcome;

//     public static ExecutionOutcome ExceptionThrown(Exception exception) => new ExceptionThrownOutcome(exception);
//     public sealed record ExceptionThrownOutcome(Exception Exception) : ExecutionOutcome;

//     public static ExecutionOutcome Unshrinkable(string reason) => new UnshrinkableOutcome(reason);
//     public sealed record UnshrinkableOutcome(string Reason) : ExecutionOutcome;

//     public static ExecutionOutcome EarlyExit => new EarlyExitOutcome();
//     public sealed record EarlyExitOutcome() : ExecutionOutcome;

//     public bool Failed =>
//         this switch
//         {
//             UnshrinkableOutcome _ => false,
//             EarlyExitOutcome => false,
//             _ => true
//         };

//     public Exception? GetException =>
//         this switch
//         {
//             ExceptionThrownOutcome a => a.Exception,
//             _ => null
//         };

//     public string? GetFailingSpec =>
//         this switch
//         {
//             SpecFailedOutcome a => a.SpecName,
//             _ => null
//         };
// }

// public class PhaseContext
// {
//     public string? FailingSpec => Outcome?.GetFailingSpec;
//     public Exception? Exception => Outcome?.GetException;
//     private readonly QAcidPhase phase;
//     public ExecutionOutcome? Outcome { get; private set; }
//     public PhaseContext(QAcidPhase phase) => this.phase = phase;

//     public void Reset()
//     {
//         Outcome = null;
//     }
//     public bool Failed => Outcome is not null && Outcome.Failed;
//     public bool ShouldStop => Outcome is not null;

//     public void StopRun() => Outcome = ExecutionOutcome.EarlyExit;

//     public void MarkFailure(string specName)
//     {
//         if (Outcome is null)
//             Outcome = ExecutionOutcome.SpecFailed(specName);
//     }

//     public void MarkFailure(Exception exception, PhaseContext originalPhase)
//     {
//         if (phase != QAcidPhase.NormalRun)
//         {
//             if (IsExceptionMismatch(exception, originalPhase))
//             {
//                 Outcome = ExecutionOutcome.Unshrinkable("Mismatched exception type");
//                 return;
//             }
//             return;
//         }
//         Outcome = ExecutionOutcome.ExceptionThrown(exception);
//     }
//     private static bool IsExceptionMismatch(Exception exception, PhaseContext original)
//     {
//         return original.Outcome is not ExecutionOutcome.ExceptionThrownOutcome o
//             || o.Exception?.GetType() != exception.GetType();
//     }
// }
