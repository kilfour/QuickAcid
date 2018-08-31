using System;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests
{
    public class ActExceptionTestsNoShrinking
    {
        [Fact]
        public void SimpleExceptionThrown()
        {
            var test =
                from foo in "foo".Act(() => throw new Exception())
                select Unit.Instance;

            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, 1));
            var report = ex.AcidReport;

            Assert.Equal(1, report.Entries.Count);

            var entry = Assert.IsType<AcidReportActEntry>(report.Entries[0]);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnSecond()
        {
            var test =
                from foo in "foo".Act(() => { })
                from bar in "bar".Act(() => throw new Exception())
                select Unit.Instance;

            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, 1));
            var report = ex.AcidReport;

            Assert.Equal(2, report.Entries.Count);

            var entry = Assert.IsType<AcidReportActEntry>(report.Entries[0]);
            Assert.Equal("foo", entry.Key);
            Assert.Null(entry.Exception);

            entry = Assert.IsType<AcidReportActEntry>(report.Entries[1]);
            Assert.Equal("bar", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnFirst()
        {
            var test =
                from foo in "foo".Act(() => throw new Exception())
                from bar in "bar".Act(() => { })
                select Unit.Instance;

            var ex = Assert.Throws<FalsifiableException>(() => test.Verify(1, 1));
            var report = ex.AcidReport;

            Assert.Equal(1, report.Entries.Count);

            var entry = Assert.IsType<AcidReportActEntry>(report.Entries[0]);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}
