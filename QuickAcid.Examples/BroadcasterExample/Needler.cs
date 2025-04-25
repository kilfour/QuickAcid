namespace QuickAcid.Examples.BroadcasterExample;


public class Needler
{
    public bool ThreadSwitch;
    public Thread? Thread;
    public Exception? ExceptionFromThread;

    public void Start(Action action)
    {
        ThreadSwitch = true;
        ExceptionFromThread = null;
        Thread = new Thread(() => ExceptionFromThread = GetExceptionThrownBy(action));
        Thread.Start();
    }
    public void Stop()
    {
        Thread!.Join();
        Thread = null;
        ThreadSwitch = false;
    }
    private static Exception? GetExceptionThrownBy(Action yourCode)
    {
        try { yourCode(); }
        catch (Exception e) { return e; }
        return null;
    }
}
