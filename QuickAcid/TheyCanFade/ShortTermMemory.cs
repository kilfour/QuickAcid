namespace QuickAcid.TheyCanFade;

public class ShortTermMemory
{
    public int ExecutionId { get; } = -1;

    public HashSet<string> ActionKeys { get; set; } = [];

    public Access Access { get; set; } = new();
}
