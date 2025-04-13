namespace QuickAcid.Examples.Tutorial.Chapter2;

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
                .AlwaysReported(K.TheDie, () => new LoadedDie())
                .AlwaysReported(K.TheObserver, () => new HashSet<int>())
                .Do("Roll", c => c.Get(K.TheObserver).Add(c.Get(K.TheDie).Roll()))
                // .FinalSpec("Die rolls 1", c => c.Get(K.TheObserver).Contains(1))
                // .FinalSpec("Die rolls 2", c => c.Get(K.TheObserver).Contains(2))
                // .FinalSpec("Die rolls 3", c => c.Get(K.TheObserver).Contains(3))
                // .FinalSpec("Die rolls 4", c => c.Get(K.TheObserver).Contains(4))
                // .FinalSpec("Die rolls 5", c => c.Get(K.TheObserver).Contains(5))
                // .FinalSpec("Die rolls 6", c => c.Get(K.TheObserver).Contains(6))
                .DumpItInAcid()
                .AndCheckForGold(10, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}