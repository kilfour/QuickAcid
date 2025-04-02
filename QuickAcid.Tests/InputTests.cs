using QuickAcid.Tests.ZheZhools;
using QuickMGenerate;

namespace QuickAcid.Tests
{
    public class InputTests
    {
        [Fact]
        public void UnusedInputIsNotReported()
        {
            var run =
                AcidTestRun.FailedRun(
                    from input in "input".ShrinkableInput(MGen.Int())
                    from foo in "foo".Act(() =>
                    {
                        if (true) throw new Exception();
                    })
                    select Acid.Test);

            run.NumberOfReportEntriesIs(1);

            var actEntry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
            Assert.Equal("foo", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }
    }
}