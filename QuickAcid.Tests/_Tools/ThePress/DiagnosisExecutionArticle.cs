using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class DiagnosisExecutionArticle(DiagnosisExecutionDeposition deposition)
    : AbstractArticle<DiagnosisExecutionDeposition>(deposition)
{
    public int DiagnosisCount => deposition.DiagnosisDepositions.Count;

    [StackTraceHidden]
    public DiagnosisArticle Diagnosis(int number)
    {
        return new DiagnosisArticle(deposition.DiagnosisDepositions[number - 1]);
    }
}
