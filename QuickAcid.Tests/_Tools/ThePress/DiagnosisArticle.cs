using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class DiagnosisArticle(DiagnosisDeposition deposition) : AbstractArticle<DiagnosisDeposition>(deposition)
{
    public int TraceCount => deposition.Traces.Count;

    [StackTraceHidden]
    public string Read(int number)
    {
        return deposition.Traces[number - 1];
    }
}