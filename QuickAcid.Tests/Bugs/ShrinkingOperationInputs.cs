using QuickAcid.Reporting;
using QuickFuzzr;
using QuickPulse.Arteries;

namespace QuickAcid.Tests.Bugs;

public class ShrinkingOperationInputs
{
    [Fact]
    public void Trying_To_Find_Failure()
    {
        var script =
            from collector in "container".Stashed(() => new TheCollector<int>())
            from ops in "ops".Choose(
                from i1 in "i1".Input(Fuzz.Constant(42))
                from a1 in "act1".Act(() => collector.Flow(i1))
                select Acid.Test,
                from i2 in "i2".Input(Fuzz.Constant(42))
                from a2 in "act2".Act(() => { if (collector.TheExhibit.Contains(i2)) { throw new Exception(); } })
                select Acid.Test
            )
            select Acid.Test;

        var ex = Assert.Throws<FalsifiableException>(() => QState.Run(script)
            .WithOneRun()
            .And(100.ExecutionsPerRun()));
        var report = ex.QAcidReport;
        Assert.Equal(2, report.OfType<ReportInputEntry>().Count());
        var entry1 = report.First<ReportInputEntry>();
        Assert.Equal("i1", entry1.Key);
        Assert.Equal("42", entry1.Value);
        var entry2 = report.Second<ReportInputEntry>();
        Assert.Equal("i2", entry2.Key);
        Assert.Equal("42", entry2.Value);
    }

    [Fact]
    public void Trying_To_Find_Failure_Again()
    {

        var script =
            from collector in "container".Stashed(() => new TheCollector<int>())
            from ops in "ops".Choose(
                from i1 in "i1".Input(Fuzz.Constant(42))
                from a1 in "act1".Act(() => collector.Flow(i1))
                select Acid.Test,
                from i2 in "i2".Input(Fuzz.Constant(42))
                let _ = i2
                from a2 in "act2".ActCarefully(() =>
                {
                    if (i2 != 42) return;
                    if (collector.TheExhibit.Contains(44)) throw new Exception();
                    if (collector.TheExhibit.Contains(43)) { collector.Flow(i2 + 2); return; }
                    collector.Flow(i2 + 1);
                })
                from spec in "does not throw".Spec(() => !a2.Threw)
                select Acid.Test
            )
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() => QState.Run(script)
            .WithOneRun()
            .And(100.ExecutionsPerRun()));
        var report = ex.QAcidReport;
        Assert.Equal(3, report.OfType<ReportInputEntry>().Count());
        var entry1 = report.First<ReportInputEntry>();
        Assert.Equal("i2", entry1.Key);
        Assert.Equal("42", entry1.Value);
        var entry2 = report.Second<ReportInputEntry>();
        Assert.Equal("i2", entry2.Key);
        Assert.Equal("42", entry2.Value);
        var entry3 = report.Third<ReportInputEntry>();
        Assert.Equal("i2", entry3.Key);
        Assert.Equal("42", entry3.Value);
    }

    [Fact]
    public void Trying_To_Find_Failure_Again_Carefully()
    {
        var script =
            from collector in "container".Stashed(() => new TheCollector<int>())
            from ops in "ops".Choose(
                from i1 in "i1".Input(Fuzz.Constant(42))
                from a1 in "act1".Act(() => collector.Flow(i1))
                select Acid.Test,
                from i2 in "i2".Input(Fuzz.Constant(42))
                let _ = i2
                from a2 in "act2".ActCarefully(() =>
                {
                    if (i2 != 42) return;
                    if (collector.TheExhibit.Contains(44)) throw new Exception();
                    if (collector.TheExhibit.Contains(43)) { collector.Flow(i2 + 2); return; }
                    collector.Flow(i2 + 1);
                })
                from spec in "does not throw".Spec(() => !a2.Threw)
                select Acid.Test
            )
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() => QState.Run(script)
            .WithOneRun()
            .And(100.ExecutionsPerRun()));
        var report = ex.QAcidReport;
        Assert.Equal(3, report.OfType<ReportInputEntry>().Count());
        var entry1 = report.First<ReportInputEntry>();
        Assert.Equal("i2", entry1.Key);
        Assert.Equal("42", entry1.Value);
        var entry2 = report.Second<ReportInputEntry>();
        Assert.Equal("i2", entry2.Key);
        Assert.Equal("42", entry2.Value);
        var entry3 = report.Third<ReportInputEntry>();
        Assert.Equal("i2", entry3.Key);
        Assert.Equal("42", entry3.Value);
    }
}