using QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.Phasers;

namespace StringExtensionCombinators;

public static partial class QAcidCombinators
{
    public record DiagnoseArgs(QAcidPhase Phase, int ExecutionId);
    public static QAcidScript<Acid> Diagnose(
        this string key,
        Func<DiagnoseArgs, string> diagnosis) => DiagnoseIf(key, a => true, diagnosis);

    public static QAcidScript<Acid> DiagnoseIf(
    this string key,
    Func<DiagnoseArgs, bool> predicate,
    Func<DiagnoseArgs, string> diagnosis) =>
    state =>
    {
        var args = new DiagnoseArgs(state.Shifter.CurrentPhase, state.CurrentExecutionNumber);
        if (predicate(args))
        {
            state.CurrentExecutionContext().Diagnose(key, diagnosis(args));
            return Vessel.AcidOnly(state);
        }
        return Vessel.AcidOnly(state);
    };
}
