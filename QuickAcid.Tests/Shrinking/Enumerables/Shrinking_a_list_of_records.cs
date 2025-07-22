using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Shrinking.Enumerables;

public class Shrinking_a_list_of_records
{
    public record Person(string Name, int Age);

    [Fact]
    public void Two_records()
    {
        var counter = 41;
        var generator =
            from name in Fuzz.Constant("jos")
            from age in Fuzz.Constant(counter++)
            select new Person(name, age);

        var script =
            from input in "input".Input(generator.Many(2))
            from foo in "act".Act(() =>
            {
                if (input.Any(a => a.Age == 42)) { throw new Exception(); }
            })
            select Acid.Test;

        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Key);
        Assert.Equal("[ { Age : 42 } ]", inputEntry.Value);
        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Key);
        Assert.NotNull(report.Exception);
    }
}