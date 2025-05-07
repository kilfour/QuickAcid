using QuickAcid.Fluent;

namespace QuickAcid.Examples.Tutorial.Chapter2.SneakyBugs;

public class Test
{
    public static class K
    {
        public static QKey<LoadedDie> TheDie => QKey<LoadedDie>.New("TheDie");
        public static QKey<HashSet<int>> TheObserver => QKey<HashSet<int>>.New("TheObserver");
    }

    [Fact]
    public void Report()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(K.TheDie, () => new LoadedDie())
                .Tracked(K.TheObserver, () => new HashSet<int>())
                .Do("Roll", c => c.Get(K.TheObserver).Add(c.Get(K.TheDie).Roll()))
                .Assay("Die rolls 1", c => c.Get(K.TheObserver).Contains(1))
                .Assay("Die rolls 2", c => c.Get(K.TheObserver).Contains(2))
                .Assay("Die rolls 3", c => c.Get(K.TheObserver).Contains(3))
                .Assay("Die rolls 4", c => c.Get(K.TheObserver).Contains(4))
                .Assay("Die rolls 5", c => c.Get(K.TheObserver).Contains(5))
                .Assay("Die rolls 6", c => c.Get(K.TheObserver).Contains(6))
                .DumpItInAcid()
                .AndCheckForGold(1, 100);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}