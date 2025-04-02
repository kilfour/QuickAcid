namespace QuickAcid.Tests
{
    public class ReportFromMemoryTests
    {
        [Fact]
        public void Report_Created_From_Memory_And_Manual_Log()
        {
            // Arrange
            int currentActionId = 0;
            var memory = new Memory(() => currentActionId);

            // Simulate setting the shrunk value in the first action
            memory.ForThisAction().Set("input", 1);

            // Simulate act log entry (as would happen in state.LogReport)
            var report = new QAcidReport
            {
                ShrinkAttempts = 4
            };

            // Convert memory to input entries
            foreach (var pair in memory.GetAll())
            {
                report.AddEntry(new QAcidReportInputEntry(pair.Key)
                {
                    Value = pair.Value
                });
            }

            // Simulate the action that failed
            report.AddEntry(new QAcidReportActEntry("foo", new Exception("boom")));

            // Assert
            Assert.Equal(2, report.Entries.Count);

            var input = Assert.IsType<QAcidReportInputEntry>(report.Entries[0]);
            Assert.Equal("input", input.Key);
            Assert.Equal(1, input.Value);

            var act = Assert.IsType<QAcidReportActEntry>(report.Entries[1]);
            Assert.Equal("foo", act.Key);
            Assert.NotNull(act.Exception);
            Assert.Equal("boom", act.Exception.Message);

            var output = report.ToString();
            Assert.Contains("input = 1", output);
            Assert.Contains("Execute : foo", output);
            Assert.Contains("boom", output);
        }
    }
}