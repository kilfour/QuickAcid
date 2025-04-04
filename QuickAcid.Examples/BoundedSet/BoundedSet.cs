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
    [Fact] //(Skip = "explicit")
    public void WithPleasure()
    {
        var run =
            from maxSize in "max size".TrackedInput(() => MGen.Int(0, 50).Generate())
            from theSet in "the set".TrackedInput(() => new BoundedSet<int>(maxSize))
            from addedInts in "addedInts".TrackedInput(() => new List<int>(), l => "[" + string.Join(", ", l) + "]")
            from choose in "ops".Choose(
                from toAdd in "to add".ShrinkableInput(MGen.Int(0, 1))
                from add in "add".Act(() => { theSet.Add(toAdd); addedInts.Add(toAdd); })
                from added in "added".Spec(() => theSet.Contains(toAdd)).If(() => theSet.Count < maxSize)
                select Acid.Test,
                from toRemove in "toRemove".Input(MGen.ChooseFrom(addedInts.ToList())).If(() => addedInts.Count > 0)
                from remove in "remove".Act(() => theSet.Remove(toRemove)).If(() => theSet.Count > 0)
                from contains in "contains".Spec(() => !theSet.Contains(toRemove))
                select Acid.Test
                )
            from checkCount in "Count <= MaxSize".Spec(() => theSet.Count <= maxSize)
            select Acid.Test;

        run.Verify(50.Runs(), 20.ActionsPerRun());
    }
}