using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests.Shrinking.Primitives
{
    public class IntTests
    {
        [Fact]
        public void OneRelevantInt()
        {
            var run =
                from input1 in "input1".Shrinkable(MGen.Int(5, 7))
                from input2 in "input2".Shrinkable(MGen.Int(5, 7))
                from foo in "act".Act(() =>
                {
                    if (input1 == 6) throw new Exception();
                })
                select Acid.Test;

            var report = new QState(run).Observe(50);

            Assert.Single(report.OfType<ReportInputEntry>());

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input1", inputEntry.Key);
            Assert.Equal("6", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }

        [Fact]
        public void TwoRelevantInts()
        {
            var run =
                from input1 in "input1".Shrinkable(MGen.Int(5, 7))
                from input2 in "input2".Shrinkable(MGen.Int(5, 7))
                from foo in "act".Act(() =>
                {
                    if (input1 == 6 && input2 == 6) throw new Exception();
                })
                select Acid.Test;

            var report = new QState(run).Observe(50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input1", inputEntry.Key);
            Assert.Equal("6", inputEntry.Value);

            inputEntry = report.SecondOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input2", inputEntry.Key);
            Assert.Equal("6", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }

        [Fact(Skip = "explicit")]
        public void TwoRelevantIntsTricky()
        {
            var run =
                from input1 in "input1".Shrinkable(MGen.Int(0, 100))
                from input2 in "input2".Shrinkable(MGen.Int(0, 100))
                from foo in "act".Act(() =>
                {
                    if (input1 > 3 && input2 == 3) throw new Exception();
                })
                select Acid.Test;

            var report = new QState(run).Observe(100);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input1", inputEntry.Key);
            Assert.Equal("3", inputEntry.Value);

            inputEntry = report.SecondOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input2", inputEntry.Key);
            Assert.NotNull(inputEntry.Value);
            bool success = int.TryParse(inputEntry.Value.ToString(), out int input2FromReport);
            Assert.True(success);
            Assert.True(input2FromReport > 3);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }
    }
}