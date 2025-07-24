using QuickAcid.Tests._Tools.ThePress;
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

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);
        Assert.Equal("[ { Age : 42 } ]", inputEntry.Value);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Label);
        Assert.NotNull(article.Exception());
    }
}