# Chapter 2: Sneaky Bugs

Not all bugs are obvious. Some pass dozens of example-based tests, live happily in production, and quietly break the rules we assumed were true.

In this chapter, we’ll meet such a bug: a **loaded die** that never rolls a six.

## The System Under Test

```csharp
public class LoadedDie
{
    private readonly Random _random = new();

    public int Roll() => _random.Next(1, 6); // ⚠️ Sneaky bug: upper bound is exclusive, 6 is never rolled!
}
```

This looks fine at first glance. We’re generating numbers “between 1 and 6,” right?

Wrong.

In .NET, `Random.Next(min, max)` excludes the upper bound. So this only returns 1 through 5.

---

## The Property

We don’t just want to know if a die roll *can* be within range. We want to know:

> Over many rolls, are **all** values from 1 to 6 ever seen?

This is where QuickAcid’s `Assay(...)` feature shines. It lets us declare final, end-of-run invariants that summarize the entire system behavior.

---

## The QuickAcid Test

```csharp
public class Test
{
    public static class K
    {
        public static QKey<LoadedDie> TheDie => QKey<LoadedDie>.New("TheDie");
        public static QKey<HashSet<int>> TheObserver => QKey<HashSet<int>>.New("TheObserver");
    }

    [Fact]
    public void Report()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(K.TheDie, () => new LoadedDie())
                .Tracked(K.TheObserver, () => new HashSet<int>())
                .Do("Roll", c => c.Get(K.TheObserver).Add(c.Get(K.TheDie).Roll()))
                .Assay("Die rolls 1", c => c.Get(K.TheObserver).Contains(1))
                .Assay("Die rolls 2", c => c.Get(K.TheObserver).Contains(2))
                .Assay("Die rolls 3", c => c.Get(K.TheObserver).Contains(3))
                .Assay("Die rolls 4", c => c.Get(K.TheObserver).Contains(4))
                .Assay("Die rolls 5", c => c.Get(K.TheObserver).Contains(5))
                .Assay("Die rolls 6", c => c.Get(K.TheObserver).Contains(6))
                .DumpItInAcid()
                .AndCheckForGold(1, 100);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}
```

---

## A Failing Case

```text
 ----------------------------------------
 -- The Assayer disagrees: Die rolls 6.
 ----------------------------------------
```

This is QuickAcid's final judgment after a full test run. The test ran 100 executions, but **6 was never observed**. That tells us everything we need to know:

- The die **can** roll 1 through 5
- But 6 is **unreachable** — and so the system is flawed

---

## Why `Assay(...)` Matters

Most property-based frameworks check individual steps. But QuickAcid lets us also define **what should be true at the end** — like a summary report from the system under test.

This makes it perfect for testing:
- Coverage (did we observe all values?)
- Accumulated state (did something get tracked?)
- Long-run guarantees (was a flag eventually set?)

The Assayer doesn’t interrupt the test. It waits.
Then — only if everything else passed — it quietly tells you:

> “Something’s missing.”