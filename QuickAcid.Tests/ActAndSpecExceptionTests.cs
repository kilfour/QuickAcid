using System;
using QuickAcid.Tests.ZheZhools;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests
{
    public class ActAndSpecExceptionTests
    {
        [Fact]
        public void ExceptionThrown()
        {
            var run =
                AcidTestRun.FailedRun(
                    from foo in "foo".Act(() => { if (true) throw new Exception();})
                    from spec in "spec".Spec(() => true)
                    select Unit.Instance);

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<AcidReportActEntry>(0);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}