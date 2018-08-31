using System;
using QuickAcid.Tests.ZheZhools;
using Xunit;

namespace QuickAcid.Tests
{
    public class SpecTests
    {
        [Fact]
        public void SpecOnlyReturnsTrue()
        {
            AcidTestRun.SuccessfullRun("foo".Spec(() => true));
        }

        [Fact]
        public void SpecOnlyReturnsFalse()
        {
            var run = AcidTestRun.FailedRun("foo".Spec(() => false));

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<QAcidReportSpecEntry>(0);
            Assert.Equal("foo", entry.Key);
        }
    }
}