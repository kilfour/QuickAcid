using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class PassedSpecsArticle : AbstractArticle<PassedSpecsDeposition>
{
    public PassedSpecsArticle(PassedSpecsDeposition deposition)
        : base(deposition) { }

    [StackTraceHidden]
    public PassedSpecArticle PassedSpec(int number)
    {
        return new PassedSpecArticle(deposition.PassedSpecDepositions[number - 1]);
    }
}

