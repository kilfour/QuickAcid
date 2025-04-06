using QuickAcid.Reporting;

namespace QuickAcid.Tests
{
    public class SpecTests
    {
        [Fact]
        public void SpecOnlyReturnsTrue()
        {
            Assert.Null("foo".Spec(() => true).ReportIfFailed());
        }

        [Fact]
        public void SpecOnlyReturnsFalse()
        {
            var report = "foo".Spec(() => false).ReportIfFailed();
            var entry = report.Entries.OfType<QAcidReportSpecEntry>().FirstOrDefault();
            Assert.NotNull(entry);
            Assert.Equal("foo", entry.Key);
        }

        [Fact]
        public void SpecMultipleFirstFails()
        {
            var run =
                from __a in "foo".Act(() => { })
                from _s1 in "first failed".Spec(() => false)
                from _s2 in "second passed".Spec(() => true)
                select Acid.Test;

            var entry = run.ReportIfFailed().Single<QAcidReportSpecEntry>();

            Assert.NotNull(entry);
            Assert.Equal("first failed", entry.Key);
        }

        [Fact]
        public void SpecMultipleSecondFails()
        {
            var run =
                from __a in "foo".Act(() => { })
                from _s1 in "first passed".Spec(() => true)
                from _s2 in "second failed".Spec(() => false)
                select Acid.Test;

            var entry = run.ReportIfFailed().Single<QAcidReportSpecEntry>();

            Assert.NotNull(entry);
            Assert.Equal("second failed", entry.Key);
        }
    }
}