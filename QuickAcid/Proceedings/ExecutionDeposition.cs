

namespace QuickAcid.Proceedings;

public record ExecutionDeposition
{
    public int ExecutionId { get; }

    public int Times { get; private set; } = 1;

    public List<ActionDeposition> ActionDepositions { get; } = [];

    public List<TrackedDeposition> TrackedDepositions { get; } = [];

    public List<InputDeposition> InputDepositions { get; } = [];

    public List<TraceDeposition> TraceDepositions { get; } = [];

    public ExecutionDeposition(int executionId)
    {
        ExecutionId = executionId;
    }

    public bool Collapsed(ExecutionDeposition other)
    {
        if (!TrackedDepositions.SequenceEqual(other.TrackedDepositions))
            return false;
        if (!ActionDepositions.SequenceEqual(other.ActionDepositions))
            return false;
        if (!InputDepositions.SequenceEqual(other.InputDepositions))
            return false;
        if (!TraceDepositions.SequenceEqual(other.TraceDepositions))
            return false;
        Times++;
        return true;
    }

    public bool HasContent()
    {
        if (TrackedDepositions.Count > 0) return true;
        if (ActionDepositions.Count > 0) return true;
        if (InputDepositions.Count > 0) return true;
        if (TraceDepositions.Count > 0) return true;
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

    public ExecutionDeposition AddTraceDeposition(TraceDeposition traceDeposition)
    {
        TraceDepositions.Add(traceDeposition);
        return this;
    }
};