namespace QuickAcid.Bolts;

public class QAcidDelayedResult
{
    public bool Threw { get; init; }
    public Exception? Exception { get; init; }

    public QAcidDelayedResult()
    {
        Threw = false;
        Exception = null;
    }

    public QAcidDelayedResult(Exception exception)
    {
        Threw = true;
        Exception = exception;
    }

    public bool Throws<TException>() where TException : Exception
        => Threw && Exception is TException;
}

public sealed class QAcidDelayedResult<T> : QAcidDelayedResult
{
    public T? Value { get; }

    public QAcidDelayedResult(T value) : base()
    {
        Value = value;
    }

    public QAcidDelayedResult(Exception exception) : base(exception)
    {
        Value = default;
    }
}
