namespace QuickAcid.TheyCanFade;

public class InputTrackerPerExecution
{
    private List<string> InputKeysAlreadyTried { get; set; } = [];

    public void MarkAsTriedToShrink(string key)
    {
        InputKeysAlreadyTried.Add(key);
    }

    public bool AlreadyTried(string key)
    {
        return InputKeysAlreadyTried.Contains(key);
    }
}

