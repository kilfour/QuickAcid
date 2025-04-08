using QuickAcid.Reporting;
using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid.Tests
{
    public class ActExceptionTests
    {
        [Fact]
        public void SimpleExceptionThrown()
        {
            var run = "foo".Act(() => { if (true) throw new Exception(); });
            var entry = run.ReportIfFailed().FirstOrDefault<ReportActEntry>();
            Assert.NotNull(entry);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnFirst()
        {
            var run =
                from foo in "foo".Act(() => throw new Exception())
                from bar in "bar".Act(() => { })
                select Acid.Test;
            var entry = run.ReportIfFailed().FirstOrDefault<ReportActEntry>();
            Assert.NotNull(entry);
            Assert.Equal("foo", entry.Key);
            Assert.NotNull(entry.Exception);
        }

        [Fact]
        public void TwoActionsExceptionThrownOnSecond()
        {
            var run =
                from foo in "foo".Act(() => { })
                from bar in "bar".Act(() => throw new Exception())
                select Acid.Test;
            var entry = run.ReportIfFailed().FirstOrDefault<ReportActEntry>();
            Assert.NotNull(entry);
            Assert.Equal("bar", entry.Key);
            Assert.NotNull(entry.Exception);
        }
    }
}
