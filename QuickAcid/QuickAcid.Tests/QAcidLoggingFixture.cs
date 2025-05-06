namespace QuickAcid.Tests
{
    public class QAcidLoggingFixture : IDisposable
    {
        public QAcidLoggingFixture()
        {
            // Enable logging when test starts
            QAcidDebug.EnableFileLogging();
        }

        public void Dispose()
        {
            // Disable after test finishes
            QAcidDebug.Disable();
        }
    }
}