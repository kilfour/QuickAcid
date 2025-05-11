using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests.Shrinking.Primitives
{
    public class CharTests
    {
        [Fact]
        public void OneRelevantChar()
        {
            var script =
                from input1 in "input1".Input(MGen.ChooseFrom(['X', 'Y']))
                from input2 in "input2".Input(MGen.Char())
                from foo in "act".Act(() =>
                {
                    if (input1 == 'X') throw new Exception("Boom");
                })
                select Acid.Test;

            var report = new QState(script).Observe(50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input1", inputEntry.Key);
            Assert.Equal("X", inputEntry.Value); // Could use .ToString(CultureInfo.InvariantCulture) if needed

            var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(report.Exception);
        }
    }
}