namespace QuickAcid.Bolts;

public class ShrinkableInputsTracker
{
    private readonly Func<int> getCurrentActionId;
    public Dictionary<int, ShrinkableInputsTrackerPerExecution> MemoryPerExecution { get; set; }

    public ShrinkableInputsTracker(Func<int> getCurrentActionId)
    {
        this.getCurrentActionId = getCurrentActionId;
        MemoryPerExecution = [];
    }

    public ShrinkableInputsTrackerPerExecution ForThisExecution()
    {
        if (!MemoryPerExecution.ContainsKey(getCurrentActionId()))
            MemoryPerExecution[getCurrentActionId()] = new ShrinkableInputsTrackerPerExecution();
        return MemoryPerExecution[getCurrentActionId()];
    }
}

public class ShrinkableInputsTrackerPerExecution
{
    private List<string> ShrinkableInputKeysAlreadyTried { get; set; } = [];

    public void MarkAsTriedToShrink(string key)
    {
        ShrinkableInputKeysAlreadyTried.Add(key);
    }

    public bool AlreadyTried(string key)
    {
        return ShrinkableInputKeysAlreadyTried.Contains(key);
    }
}

