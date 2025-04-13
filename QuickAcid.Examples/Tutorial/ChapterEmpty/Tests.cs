using QuickMGenerate;

namespace QuickAcid.Examples.Tutorial.Chapter;

public class Test
{
    public static class K
    {
        public static QKey<int> Counter => QKey<int>.New("A Key");
    }

    // [Fact]
    // public void Report()
    // {
    //     var report =
    //         SystemSpecs.Define()
    //             .Fuzzed("denominator", MGen.Int(-10, 10), x => x != 0)
    //             .As("divide"), ctx => Divide(100, ctx.Get("denominator")))
    //             .DumpItInAcid()
    //             .AndCheckForGold(10, 10);
    //     if (report != null)
    //         Assert.Fail(report.ToString());
    // }
}