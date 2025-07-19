

namespace QuickAcid.Proceedings;

public class ExecutionDeposition
{
    public int ExecutionId { get; }

    public List<ActionDeposition> ActionDepositions { get; } = [];

    public List<TrackedDeposition> TrackedDepositions { get; } = [];

    public List<InputDeposition> InputDepositions { get; } = [];

    public ExecutionDeposition(int executionId)
    {
        ExecutionId = executionId;
    }

    public bool HasContent()
    {
        if (ActionDepositions.Count > 0) return true;
        if (TrackedDepositions.Count > 0) return true;
        if (InputDepositions.Count > 0) return true;
        return false;
    }

    public ExecutionDeposition AddActionDeposition(ActionDeposition actionDeposition)
    {
        ActionDepositions.Add(actionDeposition);
        return this;
    }

    public ExecutionDeposition AddTrackedDeposition(TrackedDeposition trackedDeposition)
    {
        TrackedDepositions.Add(trackedDeposition);
        return this;
    }

    public ExecutionDeposition AddInputDeposition(InputDeposition inputDeposition)
    {
        InputDepositions.Add(inputDeposition);
        return this;
    }
};