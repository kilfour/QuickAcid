using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;

namespace QuickAcid.Tests
{
    public class ActAndInputExceptionTests
    {
        [Fact]
        public void ExceptionThrownByAct()
        {
            var run =
                from input in "input".ShrinkableInput(MGen.Int(1, 1))
                from foo in "foo".Act(() => { if (input == 1) throw new Exception(); })
                from spec in "spec".Spec(() => true)
                select Acid.Test;

            var report = run.ReportIfFailed();

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("foo", actEntry.Key);
        }
    }
}