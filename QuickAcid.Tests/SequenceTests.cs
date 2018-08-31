using System;
using QuickAcid.Tests.ZheZhools;
using Xunit;

namespace QuickAcid.Tests
{
    public class SequenceTests
    {
        [Fact]
        public void TwoActionsExceptionThrownOnFirst()
        {
            var run =
                AcidTestRun.FailedRun(
                    "foobar".Sequence(
                        "foo".Act(() => throw new Exception() ),
                        "bar".Act(() => { })));

            run.NumberOfReportEntriesIs(1);
            
            var entry = run.GetReportEntryAtIndex<AcidReportActEntry>(0);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnSecond()
        {
            var run =
                AcidTestRun.FailedRun(2,
                    "foobar".Sequence(
                        "foo".Act(() => { }),
                        "bar".Act(() => throw new Exception())));

            run.NumberOfReportEntriesIs(1);

            var entry = run.GetReportEntryAtIndex<AcidReportActEntry>(0);
            Assert.Equal("bar", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}