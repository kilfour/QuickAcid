using QuickAcid.Fluent;

using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Fluent.FuzzedInput;

public class FuzzedInputTests
{
    public static class Keys
    {
        public static readonly QKey<Container> Container =
            QKey<Container>.New("container");
        public static QKey<Container> Universe =>
            QKey<Container>.New("universe");
        public static QKey<int> TheAnswer =>
            QKey<int>.New("theAnswer");
        public static QKey<int> TheWrongAnswer =>
            QKey<int>.New("TheWrongAnswer");
    }

    public class Container
    {
        public int ItsOnlyAModel { get; set; }
        public override string ToString()
        {
            return ItsOnlyAModel.ToString();
        }
    }

    [Fact]
    public void FuzzedInput_should_exist_in_per_action_report_if_relevant()
    {
        var report =
            SystemSpecs
                .Define()
                .Fuzzed(Keys.TheAnswer, MGen.Constant(2))
                .As("throw")
                    .UseThe(Keys.TheAnswer)
                    .Now(i => { if (i == 2) { throw new Exception(); } })
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("theAnswer", inputEntry.Key);
        Assert.Equal("2", inputEntry.Value);
    }

    [Fact]
    public void FuzzedInput_should_not_exist_in_per_action_report_if_irrelevant()
    {
        var report =
            SystemSpecs
                .Define()
                .Fuzzed(Keys.TheAnswer, MGen.Constant(2))
                .As("throw")
                    .UseThe(Keys.TheAnswer)
                    .Now(_ => throw new Exception())
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.Null(inputEntry);
    }

    [Fact]
    public void FuzzedInput_can_use_context()
    {
        var theAnswer = 0;
        var report =
            SystemSpecs.Define()
                .Fuzzed(Keys.TheWrongAnswer, MGen.Constant(41))
                .Fuzzed(Keys.TheAnswer, ctx => MGen.Constant(ctx.Get(Keys.TheWrongAnswer) + 1))
                .As("Answering the Question")
                    .UseThe(Keys.TheAnswer)
                    .Now(answer => theAnswer = answer)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal(42, theAnswer);
    }
}