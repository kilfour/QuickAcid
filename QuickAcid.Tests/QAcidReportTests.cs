using QuickAcid.Reporting;

namespace QuickAcid.Tests
{
    public class QAcidReportTests
    {
        [Fact]
        public void Report_ShouldInclude_Input_And_Act_With_Exception()
        {
            // Arrange
            var report = new QAcidReport();
            report.ShrinkAttempts = 5;

            report.AddEntry(new QAcidReportInputEntry("input")
            {
                Value = 1
            });

            report.AddEntry(new QAcidReportActEntry("foo", new Exception("boom")));

            // Act
            var entries = report.Entries;

            // Assert
            Assert.Equal(2, entries.Count);

            var inputEntry = Assert.IsType<QAcidReportInputEntry>(entries[0]);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal(1, inputEntry.Value);

            var actEntry = Assert.IsType<QAcidReportActEntry>(entries[1]);
            Assert.Equal("foo", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
            Assert.Equal("boom", actEntry.Exception.Message);

            var str = report.ToString();
            Assert.Contains("Execute : foo", str);
            Assert.Contains("Input : input = 1", str);
            Assert.Contains("boom", str);
        }
    }
}