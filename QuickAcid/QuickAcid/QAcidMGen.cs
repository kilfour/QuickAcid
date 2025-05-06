using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;

namespace QuickAcid;

public static partial class QAcidMGen
{
    public static Generator<T> Claim<T>(this Generator<T> generator, Func<T, bool> predicate)
    {
        var filtered = generator.Where(predicate);
        return new ClaimedGenerator<T>(filtered, predicate);
    }
}
