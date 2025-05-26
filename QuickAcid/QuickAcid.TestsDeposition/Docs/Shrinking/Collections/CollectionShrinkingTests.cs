using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.Collections;

public static class Chapter { public const string Order = "1-50-7"; }

[Doc(Order = Chapter.Order, Caption = "Collection Shrinking", Content =
@"...
")]
public class CollectionShrinkingTests
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1", Content = @"Usage")]
    public void Collection_shrink()
    {
        var script =
            from input in "input".Input(MGen.Constant<IEnumerable<int>>([1, 2, 3]))
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => input.Count() <= 2)
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.NotNull(report);
        var entry = report.Single<ReportInputEntry>();
        Assert.Equal("[ _, _, _ ]", entry.Value);
    }

    [Fact]
    public void Collection_nested_shrink()
    {
        var script =
            from input in "input".Input(MGen.Constant<IEnumerable<List<int>>>([[42, 1], [2, 3]]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(
                () => input.Any() && input.First().Any(),
                () => input.First().First() != 42)
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.NotNull(report);
        var entry = report.Single<ReportInputEntry>();
        Assert.Equal("[ [ 42 ] ]", entry.Value);
    }

    [Fact]
    public void Collection_shrink_with_extra()
    {
        var script =
            from input in "input".Input(MGen.Constant<IEnumerable<int>>([1, 2, 42]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(() => input.Count() > 2, () => input.ToList()[2] != 42)
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.NotNull(report);
        var entry = report.Single<ReportInputEntry>();
        Assert.Equal("[ _, _, 42 ]", entry.Value);
    }

    public Generator<IEnumerable<int>> GrowingListUp()
    {
        var counter = 3;
        return
            state =>
                {
                    var list = new List<int>();
                    var i = 1;
                    counter.Times(() => list.Add(i++));
                    return new Result<IEnumerable<int>>(list, state);
                };
    }

    [Fact]
    public void Collection_shrink_with_dep()
    {
        var script =
            from input in "input".Input(MGen.Constant<IEnumerable<int>>([1, 2, 3]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(() => input.Count() > 2, () =>
            !(input.ToList()[0] == 1
            && input.ToList()[1] == 2
            && input.ToList()[2] == 3))
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.NotNull(report);
        var entry = report.Single<ReportInputEntry>();
        Assert.Equal("[ 1, 2, 3 ]", entry.Value);
    }

    [Fact]
    public void Collection_irrelevant_not_in_report()
    {
        var script =
            from input in "input".Input(MGen.Int().Many(3))
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.NotNull(report);
        Assert.Null(report.FirstOrDefault<ReportInputEntry>());
    }

    [Fact]
    public void Collection_shrink_with_haha()
    {
        // TODO : MAKE IT PASS WITH : !input.Contains(1)
        var script =
            from input in "input".Input(MGen.Constant<IEnumerable<int>>([1, 2, 1]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(() => input.Count() > 2, () => !input.Contains(1))
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.NotNull(report);
        var entry = report.Single<ReportInputEntry>();
        Assert.Equal("[ _, 2, _ ]", entry.Value);
    }
}