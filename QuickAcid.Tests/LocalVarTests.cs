using QuickAcid.Tests.ZheZhools;
using QuickMGenerate;

namespace QuickAcid.Tests
{
    public class LocalVarTests
    {
        [Fact]
        public void LocalVarIsNotReported()
        {
            var run =
                AcidTestRun.FailedRun(
                    from input in "input".Input(MGen.Int())
                    from localVar in "local".LocalVar(() => 1)
                    from foo in "foo".Act(() => { })
                    from spec in "spec".Spec(() => input + 1 == input)
                    select Acid.Test);

            run.NumberOfReportEntriesIs(3);

            var inputEntry = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
            Assert.Equal("input", inputEntry.Key);

            var actEntry = run.GetReportEntryAtIndex<QAcidReportActEntry>(1);
            Assert.Equal("foo", actEntry.Key);

            var specEntry = run.GetReportEntryAtIndex<QAcidReportSpecEntry>(2);
            Assert.Equal("spec", specEntry.Key);
        }
    }
}