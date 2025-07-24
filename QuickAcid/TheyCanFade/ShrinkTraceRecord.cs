using QuickAcid.Bolts.ShrinkStrats;
using QuickPulse.Show;

namespace QuickAcid.Bolts.TheyCanFade;

public enum ShrinkKind { PrimitiveKind, ObjectKind, EnumerableKind, KeepSameKind }

public class ShrinkTraceRecord
{
    public ShrinkKind ShrinkKind { get; set; }
    public List<ShrinkTrace> Traces = [];
    public Dictionary<string, ShrinkTraceRecord> Children = [];

    private ShrinkTrace? GetFinalTrace()
    {
        if (Traces.Any(a => a.IsRemoved))
        {
            var primitiveTrace = Traces.FirstOrDefault(a => a.IsKeep && a.Strategy == "PrimitiveShrink");
            if (primitiveTrace != null)
                return primitiveTrace;
            return Traces.First(a => a.IsRemoved);
        }
        if (Traces.Any(a => a.IsIrrelevant))
        {
            var primitiveTrace = Traces.FirstOrDefault(a => a.IsKeep && a.Strategy == "PrimitiveShrink");
            if (primitiveTrace != null)
                return primitiveTrace;
            return Traces.First(a => a.IsIrrelevant);
        }
        if (Traces.Any(a => a.IsReplacement))
            return Traces.First(a => a.IsReplacement);
        return Traces.FirstOrDefault();
    }

    private static string ShrinkTraceToString(ShrinkTrace? shrinkTrace, string irrelevantString)
    {
        if (shrinkTrace == null)
            return string.Empty;
        if (shrinkTrace.IsRemoved)
            return string.Empty;
        if (shrinkTrace.IsIrrelevant)
            return irrelevantString;
        if (shrinkTrace.IsReplacement)
            return Introduce.This(shrinkTrace.Result!, false);
        if (shrinkTrace.IsKeep)
            return Introduce.This(shrinkTrace.Original!, false);
        return string.Empty;
    }

    public string GetShrinkReportString(string irrelevantString = "")
    {
        if (ShrinkKind == ShrinkKind.PrimitiveKind)
        {
            return ShrinkTraceToString(GetFinalTrace(), irrelevantString);
        }

        if (ShrinkKind == ShrinkKind.EnumerableKind)
        {
            if (Traces.Any(a => a.IsIrrelevant))
                return "";
            if (Traces.Any(a => a.IsRemoved))
                return "";
            var replaced = Traces.FirstOrDefault(a => a.IsReplacement);
            if (replaced != null)
                return Introduce.This(replaced.Result!, false);
            var shrinkValues = Children.Values.Select(a => a.GetShrinkReportString("_")).Where(a => !string.IsNullOrEmpty(a));
            if (shrinkValues.Any())
                return $"[ {string.Join(", ", shrinkValues)} ]";
            return "";

        }

        if (ShrinkKind == ShrinkKind.ObjectKind)
        {
            if (Traces.Any(a => a.IsIrrelevant))
                return "";
            if (Traces.Any(a => a.IsRemoved))
                return "";
            var replaced = Traces.FirstOrDefault(a => a.IsReplacement);
            if (replaced != null)
                return Introduce.This(replaced.Result!, false);
            var shrinkValues =
                Children.Values
                    .Select(a => (a.GetFinalTrace()!.Name, Value: a.GetShrinkReportString("")))
                    .Where(a => !string.IsNullOrEmpty(a.Value))
                    .Select(a => $"{a.Name} : {a.Value!}");
            if (shrinkValues.Any())
                return $"{{ {string.Join(", ", shrinkValues)} }}";
            return "";
        }

        return "";
    }
}

