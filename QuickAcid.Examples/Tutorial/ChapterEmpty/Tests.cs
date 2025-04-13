namespace QuickAcid.Examples.Tutorial.Chapter;

public class Test
{
    public static class K
    {
        public static QKey<int> Counter => QKey<int>.New("A Key");
    }

    [Fact]
    public void Report()
    {
        var report =
            SystemSpecs.Define()
                .DumpItInAcid()
                .AndCheckForGold(10, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}