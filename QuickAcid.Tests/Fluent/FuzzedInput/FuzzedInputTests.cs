using System.Globalization;
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
    public void FuzzedInput_should_be_the_same_all_through_the_action()
    {
        var theAnswer = 0;
        var report =
            SystemSpecs
                .Define()
                .Fuzzed(Keys.TheAnswer, MGen.Int(1, 1000))
                .As("to local storage").UseThe(Keys.TheAnswer).Now(a => theAnswer = a)
                .Expect("check local storage").UseThe(Keys.TheAnswer).Ensure(a => a == theAnswer)
                .DumpItInAcid()
                .AndCheckForGold(10, 1);
        Assert.Null(report);
    }

    [Fact]
    public void FuzzedInput_should_be_the_different_per_action()
    {
        var theAnswer1 = 0;
        var theAnswer2 = 0;
        var report =
            SystemSpecs
                .Define()
                .Fuzzed(Keys.TheAnswer, MGen.Int(1, 1000))
                .As("to local storage").UseThe(Keys.TheAnswer).Now(a => theAnswer1 = a)
                .Expect("check local storage").UseThe(Keys.TheAnswer).Ensure(a => a == theAnswer1)
                .As("to local storage again").UseThe(Keys.TheAnswer).Now(a => theAnswer2 = a)
                .Expect("check local storage again").UseThe(Keys.TheAnswer).Ensure(a => a == theAnswer2)
                .Assert("differs per action", () => theAnswer1 != theAnswer2)
                .DumpItInAcid()
                .AndCheckForGold(10, 1);
        Assert.Null(report);
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