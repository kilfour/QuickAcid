namespace QuickAcid.Examples.BroadcasterExample.SimpleModel;

public interface IClientProxy
{
    event EventHandler Faulted;
    void SendNotificationAsynchronously(Notification notification);
}

public class TestClientProxy : IClientProxy
{
    public event EventHandler? Faulted;

    public List<Notification> ReceivedNotifications { get; } = new();
    public bool WasRemoved { get; set; } = false;

    public virtual void SendNotificationAsynchronously(Notification notification)
    {
        ReceivedNotifications.Add(notification);
        Thread.Sleep(100);
    }

    public void Fault()
    {
        Faulted?.Invoke(this, EventArgs.Empty);
    }

    public void MarkAsRemoved()
    {
        WasRemoved = true;
    }
}