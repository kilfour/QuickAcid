

namespace QuickAcid.Proceedings;

public record ExecutionDeposition
{
    public int ExecutionId { get; }

    public int Times { get; private set; } = 1;

    public List<ActionDeposition> ActionDepositions { get; } = [];

    public List<TrackedDeposition> TrackedDepositions { get; } = [];

    public List<InputDeposition> InputDepositions { get; } = [];

    public ExecutionDeposition(int executionId)
    {
        ExecutionId = executionId;
    }

    public bool Collapsed(ExecutionDeposition other)
    {
        if (!ElementsMatch(TrackedDepositions, other.TrackedDepositions))
            return false;
        if (!ElementsMatch(ActionDepositions, other.ActionDepositions))
            return false;
        if (!ElementsMatch(InputDepositions, other.InputDepositions))
            return false;
        Times++;
        return true;
    }
    private bool ElementsMatch<T>(List<T> one, List<T> two)
    {
        if (one.Count != two.Count) return false;
        for (int i = 0; i < one.Count; i++)
        {
            if (one[i] == null && two[i] == null) return true;
            if (one[i] == null && two[i] != null) return false;
            if (!one[i]!.Equals(two[i]))
                return false;
        }
        return true;
    }

    public bool HasContent()
    {
        if (TrackedDepositions.Count > 0) return true;
        if (ActionDepositions.Count > 0) return true;
        if (InputDepositions.Count > 0) return true;
        return false;
    }

    public ExecutionDeposition AddTrackedDeposition(TrackedDeposition trackedDeposition)
    {
        TrackedDepositions.Add(trackedDeposition);
        return this;
    }

    public ExecutionDeposition AddActionDeposition(ActionDeposition actionDeposition)
    {
        ActionDepositions.Add(actionDeposition);
        return this;
    }

    public ExecutionDeposition AddInputDeposition(InputDeposition inputDeposition)
    {
        InputDepositions.Add(inputDeposition);
        return this;
    }
};