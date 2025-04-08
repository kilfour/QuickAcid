using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;


namespace QuickAcid.Tests
{
    public class InputTests
    {
        [Fact]
        public void UnusedInputIsNotReported()
        {
            var run =
                from input in "input".ShrinkableInput(MGen.Int())
                from foo in "foo".Act(() =>
                {
                    if (true) throw new Exception();
                })
                select Acid.Test;
            var report = run.ReportIfFailed();
            var entry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(entry);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}