using System;
using QuickAcid.Tests.ZheZhools;
using QuickMGenerate;
using Xunit;

namespace QuickAcid.Tests
{
    public class ActAndInputExceptionTests
    {
        [Fact]
        public void ExceptionThrownByAct()
        {
            var run =
                AcidTestRun.FailedRun(
                    from input in "input".ShrinkableInput(MGen.Int(1,1))
                    from foo in "foo".Act(() =>
                    {
                        if (input == 1) throw new Exception();
                    })
                    from spec in "spec".Spec(() => true)
                    select Acid.Test);

            run.NumberOfReportEntriesIs(2);

            var inputEntry = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
            Assert.Equal("input", inputEntry.Key);

            var actEntry = run.GetReportEntryAtIndex<QAcidReportActEntry>(1);
            Assert.Equal("foo", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }
    }
}