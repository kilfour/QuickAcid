namespace QuickAcid.Examples
{
    public class QAcidLoggingFixture : IDisposable
    {
        public QAcidLoggingFixture()
        {
            QAcidDebug.EnableFileLogging();
        }

        public void Dispose()
        {
            QAcidDebug.Disable();
        }
    }
}