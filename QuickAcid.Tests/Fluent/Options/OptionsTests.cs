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
                        opt.Perform("1", () => { collector+= "a"; }),
                    opt.Perform("2", () => { collector+= "b"; }) ])
                    .PickOne()
                    .DumpItInAcid()
                    .AndCheckForGold(1, 2);
            outcomes.Add(collector);
        });
        Assert.Contains("ab", outcomes);
        Assert.Contains("ba", outcomes);
    }
}
