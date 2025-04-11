namespace QuickAcid.Bolts;

// -----------------------------------------------------------------------------------------------
// just thinking out silently
// --
public interface IRememberThingsAboutAnExecution { }
// -----------------------------------------------------------------------------------------------

public class ShrinkableInputsTracker // is gonna be an abstract class with tea
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

public class ShrinkableInputsTrackerPerExecution : IRememberThingsAboutAnExecution
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

