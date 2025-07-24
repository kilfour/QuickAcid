namespace QuickAcid.TheyCanFade;

public class InputTracker
{
    private readonly Func<int> getCurrentActionId;
    public Dictionary<int, InputTrackerPerExecution> MemoryPerExecution { get; set; }

    public InputTracker(Func<int> getCurrentActionId)
    {
        this.getCurrentActionId = getCurrentActionId;
        MemoryPerExecution = [];
    }

    public InputTrackerPerExecution ForThisExecution()
    {
        if (!MemoryPerExecution.ContainsKey(getCurrentActionId()))
            MemoryPerExecution[getCurrentActionId()] = new InputTrackerPerExecution();
        return MemoryPerExecution[getCurrentActionId()];
    }
}

