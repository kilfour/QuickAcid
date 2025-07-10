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

    public bool ThrewAs<TException>() where TException : Exception
        => Threw && Exception is TException;

    public bool ThrewExactly<T>() where T : Exception
    => Exception?.GetType() == typeof(T);

    public bool ThrewAs<T>(out T ex) where T : Exception
    {
        if (Threw && Exception is T matched)
        {
            ex = matched;
            return true;
        }

        ex = default!;
        return false;
    }
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
