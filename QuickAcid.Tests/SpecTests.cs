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
    }
}