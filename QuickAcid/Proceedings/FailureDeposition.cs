namespace QuickAcid.Proceedings
{
    public abstract class FailureDeposition { };

    public class ExceptionDeposition : FailureDeposition
    {
        public Exception? Exception { get; }

        public ExceptionDeposition(Exception exception)
        {
            Exception = exception;
        }
    };

    public class FailedSpecDeposition : FailureDeposition
    {
        public string FailedSpec { get; }

        public FailedSpecDeposition(string failedSpec)
        {
            FailedSpec = failedSpec;
        }
    };
}