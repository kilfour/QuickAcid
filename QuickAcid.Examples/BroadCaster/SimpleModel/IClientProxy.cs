using System;

namespace QuickAcid.Examples.BroadCaster.SimpleModel
{
    public interface IClientProxy
    {
        event EventHandler Faulted;
        void SendNotificationAsynchronously(Notification notification);
    }
}