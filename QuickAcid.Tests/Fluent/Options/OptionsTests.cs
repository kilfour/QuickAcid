using QuickAcid.Fluent;

using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Tests.Fluent.Perform;

public class OptionsTests
{
    [Fact]
    public void Options_when_running_multiple_should_randomly_choose_one()
    {
        var outcomes = new HashSet<string>();
        100.Times(() =>
        {
            var collector = "";
            var report =
               SystemSpecs.Define()
                    .Options(opt => [
                        opt.Do("1", () => { collector+= "a"; }),
                        opt.Do("2", () => { collector+= "b"; }) ])
                    .PickOne()
                    .DumpItInAcid()
                    .AndCheckForGold(1, 2);
            outcomes.Add(collector);
        });
        Assert.Contains("ab", outcomes);
        Assert.Contains("ba", outcomes);
    }

    [Fact]
    public void Options_before_should_always_run_before_every_do()
    {
        var collector = "";
        var report =
           SystemSpecs.Define()
                .Options(opt => [
                    opt.Do("1", () => { collector+= "b"; }),
                    opt.Do("2", () => { collector+= "c"; }) ])
                .Before(() => { collector += "a"; })
                .PickOne()
                .DumpItInAcid()
                .AndCheckForGold(1, 20);
        Assert.True(collector.Where((c, i) => i % 2 == 0).All(c => c == 'a'), collector);
    }
}
