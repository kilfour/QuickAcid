using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class TrackedArticle : AbstractArticle<TrackedDeposition>
{
    public TrackedArticle(TrackedDeposition deposition)
        : base(deposition) { }
}