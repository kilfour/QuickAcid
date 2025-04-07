using QuickMGenerate;

namespace QuickAcid.Examples;

public class BoundedSet<T>
{
    private readonly HashSet<T> _set = new();
    public int MaxSize { get; }

    public BoundedSet(int maxSize)
    {
        MaxSize = maxSize;
    }

    public bool Add(T item)
    {
        if (_set.Count >= MaxSize)
            return false;
        return _set.Add(item);
    }

    public bool Contains(T item) => _set.Contains(item);

    public bool Remove(T item) => _set.Remove(item);

    public int Count => _set.Count;

    public override string ToString() =>
        $"[BoundedSet( Count: {Count}, MaxSize: {MaxSize}): {string.Join(", ", _set)}]";
}

public class BoundedSetTest : QAcidLoggingFixture
{
    [Fact]
    public void WithPleasure()
    {
        var run =
            from maxSize in "max size".AlwaysReported(() => MGen.Int(0, 50).Generate())
            from theSet in "the set".AlwaysReported(() => new BoundedSet<int>(maxSize))
            from addedInts in "addedInts".AlwaysReported(() => new List<int>(), l => "[" + string.Join(", ", l) + "]")
            from choose in "ops".Choose(
                from toAdd in "to add".ShrinkableInput(MGen.Int(0, 1))
                from add in "add".Act(
                    () => { theSet.Add(toAdd); addedInts.Add(toAdd); })
                from added in "added".SpecIf(
                    () => addedInts.Count > 0 && theSet.Count < maxSize,
                    () => theSet.Contains(toAdd))
                select Acid.Test,
                from toRemove in "toRemove".InputIf(() => theSet.Count > 0, MGen.ChooseFrom(addedInts.ToList()))
                from remove in "remove".ActIf(() => theSet.Count > 0, () => theSet.Remove(toRemove))
                from contains in "contains".Spec(() => !theSet.Contains(toRemove))
                select Acid.Test
                )
            from checkCount in "Count <= MaxSize".Spec(() => theSet.Count <= maxSize)
            select Acid.Test;

        var report = run.ReportIfFailed(50, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}