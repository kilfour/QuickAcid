using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using QuickPulse.Arteries;

namespace QuickAcid.Tests.Bugs;

public class ShrinkingOperationInputs
{
    [Fact]
    public void Trying_To_Find_Failure()
    {
        var script =
            from collector in "container".Stashed(() => TheCollector.Exhibits<int>())
            from ops in Script.Choose(
                from i1 in "i1".Input(Fuzz.Constant(42))
                from a1 in "act1".Act(() => collector.Absorb(i1))
                select Acid.Test,
                from i2 in "i2".Input(Fuzz.Constant(42))
                from a2 in "act2".Act(() => { if (collector.TheExhibit.Contains(i2)) { throw new Exception(); } })
                select Acid.Test
            )
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRun()
                .And(100.ExecutionsPerRun()));

        Assert.Equal(2, article.Total().Inputs());

        var entry1 = article.Execution(1).Input(1).Read();
        Assert.Equal("i1", entry1.Label);
        Assert.Equal("42", entry1.Value);

        var entry2 = article.Execution(2).Input(1).Read();
        Assert.Equal("i2", entry2.Label);
        Assert.Equal("42", entry2.Value);
    }

    [Fact]
    public void Trying_To_Find_Failure_Again()
    {

        var script =
            from collector in "container".Stashed(() => TheCollector.Exhibits<int>())
            from ops in Script.Choose(
                from i1 in "i1".Input(Fuzz.Constant(42))
                from a1 in "act1".Act(() => collector.Absorb(i1))
                select Acid.Test,
                from i2 in "i2".Input(Fuzz.Constant(42))
                let _ = i2
                from a2 in "act2".ActCarefully(() =>
                {
                    if (i2 != 42) return;
                    if (collector.TheExhibit.Contains(44)) throw new Exception();
                    if (collector.TheExhibit.Contains(43)) { collector.Absorb(i2 + 2); return; }
                    collector.Absorb(i2 + 1);
                })
                from spec in "does not throw".Spec(() => !a2.Threw)
                select Acid.Test
            )
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRun()
                .And(100.ExecutionsPerRun()));

        Assert.Equal(3, article.Execution(1).Read().Times);

        var inputDeposition = article.Execution(1).Input(1).Read();
        Assert.Equal("i2", inputDeposition.Label);
        Assert.Equal("42", inputDeposition.Value);
    }

    [Fact]
    public void Trying_To_Find_Failure_Again_Carefully()
    {
        var script =
            from collector in "container".Stashed(() => TheCollector.Exhibits<int>())
            from ops in Script.Choose(
                from i1 in "i1".Input(Fuzz.Constant(42))
                from a1 in "act1".Act(() => collector.Absorb(i1))
                select Acid.Test,
                from i2 in "i2".Input(Fuzz.Constant(42))
                let _ = i2
                from a2 in "act2".ActCarefully(() =>
                {
                    if (i2 != 42) return;
                    if (collector.TheExhibit.Contains(44)) throw new Exception();
                    if (collector.TheExhibit.Contains(43)) { collector.Absorb(i2 + 2); return; }
                    collector.Absorb(i2 + 1);
                })
                from spec in "does not throw".Spec(() => !a2.Threw)
                select Acid.Test
            )
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRun()
                .And(100.ExecutionsPerRun()));

        Assert.Equal(3, article.Execution(1).Read().Times);

        var inputDeposition = article.Execution(1).Input(1).Read();
        Assert.Equal("i2", inputDeposition.Label);
        Assert.Equal("42", inputDeposition.Value);
    }
}