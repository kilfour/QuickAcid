using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;
using QuickMGenerate;

namespace QuickAcid.Examples.Tutorial.Chapter3.TheShortestPathtoaBug;

public class Test
{
    public static class K
    {
        public static QKey<List<string>> Commands => QKey<List<string>>.New("Commands");
    }

    [Fact(Skip = "This one Fails, ... intentionally")]
    //[Fact]
    public void CrashesOnBadInput()
    {
        var report =
            SystemSpecs.Define()
                .Fuzzed(K.Commands,
                    MGen.ChooseFromThese("SET", "GET", "DEL", "X", "Y", "42").Many(1, 4).ToList())
                .Do("reset", _ => CommandParser.Reset())
                .Do("parse", ctx =>
                {
                    var input = ctx.Get(K.Commands).ToList();
                    CommandParser.Parse(input);
                })
                .Assay("should not throw", _ => true)
                .DumpItInAcid()
                .KeepOneEyeOnTheTouchStone()
                .AndCheckForGold(1, 100);

        if (report != null)
            Assert.Fail(report.ToString());
    }
}