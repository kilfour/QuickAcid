using QuickAcid.Fluent;

namespace QuickAcid.Examples.Tutorial.Chapter1.YourFirstPropertyTest;

public class Test
{
    public static class K
    {
        public static QKey<Counter> Counter => QKey<Counter>.New("Counter(a.k.a. the Model)");
    }

    [Fact]
    public void CheckTheSpecs()
    {
        var report =
            SystemSpecs.Define()
                .Tracked("Counter(a.k.a. the Model)", () => new Counter())
                .Options(opt => [
                    opt.Do("Increment", c => c.Get(K.Counter).Increment()),
                    opt.Do("Decrement", c => c.Get(K.Counter).Decrement())
                ])
                .Assert("always positive", c => c.Get(K.Counter).Value >= 0)
                .DumpItInAcid()
                .KeepOneEyeOnTheTouchStone()
                .AndCheckForGold(10, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}