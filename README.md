# QuickAcid: Property-Based Testing for C# 

**QuickAcid** is an experimental property-based testing framework for .NET that’s built to handle **complex system behavior**, not just data transformations. It combines:

- A **fluent, readable API** (`Fuzzed(...)`, `Do(...)`, `Ensure(...)`, etc.)
- A **LINQ-composable generator engine** (`QuickMGenerate`)
- **Stateful specs** with optional keys for type-safe context access
- Built-in **shrinking**, **reporting**, and support for generating **minimal failing unit tests** in the pipeline.

Whether you’re testing a queue, a message bus, an elevator, or some gnarly business logic with edge cases and sequencing, QuickAcid tries to keep your tests expressive *and* debuggable.

Feedback is very welcome!

---

## 📚 QuickAcid Tutorial

A guided walkthrough in bite-sized chapters:

| Chapter | Title                             | Type        |
|---------|-----------------------------------|-------------|
| 1       | [Your First Property Test](https://github.com/kilfour/QuickAcid/blob/master/QuickAcid.Examples/Tutorial/Chapter1.YourFirstPropertyTest/About.md) | ✅ Test + Markdown |
| 2       | [The Assayer Disagrees](https://github.com/kilfour/QuickAcid/blob/master/QuickAcid.Examples/Tutorial/Chapter2.SneakyBugs/About.md)       | ✅ Test + Markdown |
| 3       | [The Shortest Path to a Bug](https://github.com/kilfour/QuickAcid/blob/master/QuickAcid.Examples/Tutorial/Chapter3.TheShortestPathtoaBug/About.md) | ✅ Test + Markdown |
| 4       | [Controlling the Chaos](https://github.com/kilfour/QuickAcid/blob/master/QuickAcid.Examples/Tutorial/Chapter4.ControllingTheChaos/About.md)        | 📄 Markdown only   |

---

## Example

```csharp
public static class K
{
    public static QKey<ICollection<int>> Set => QKey<ICollection<int>>.New("Set");
    public static QKey<int> IntToAdd => QKey<int>.New("IntToAdd");
    public static QKey<int> IntToRemove => QKey<int>.New("IntToRemove");
}

public class SetTest
{
    [Fact]
    public void ReportsError()
    {
        var report =
            SystemSpecs.Define()
                .AlwaysReported(K.Set, () => new List<int>())
                .Fuzzed(K.IntToAdd, MGen.Int(1, 10))
                .Fuzzed(K.IntToRemove, MGen.Int(1, 10))
                .Options(opt => [
                    opt.Do("Add", c => c.Get(K.Set).Add(c.Get(K.IntToAdd)))
                        .Expect("Set contains added int")
                        .Ensure(ctx => ctx.Get(K.Set).Contains(ctx.Get(K.IntToAdd))),
                    opt.Do("Remove", c => c.Get(K.Set).Remove(c.Get(K.IntToRemove)))
                        .Expect("Set does not contain removed int")
                        .Ensure(ctx => !ctx.Get(K.Set).Contains(ctx.Get(K.IntToRemove)))
                ])
                .PickOne()
                .DumpItInAcid()
                .AndCheckForGold(30, 50);

        if (report != null)
            Assert.Fail(report.ToString());
    }
}
```

### Sample Output

```
 ----------------------------------------
 -- Falsified after 3 actions, 12 shrinks
 ----------------------------------------
 RUN START :
   => Set (tracked) : [  ]
 ---------------------------
 EXECUTE : Add
   - Input : IntToAdd = 4
   => Set (tracked) : [ 4 ]
 ---------------------------
 EXECUTE : Add
   - Input : IntToAdd = 4
   => Set (tracked) : [ 4, 4 ]
 ---------------------------
 EXECUTE : Remove
   - Input : IntToRemove = 4

 ********************************
  Spec Failed : Set does not contain removed int
 ********************************
```

---

## Why Keys?

Keys like `QKey<T>` are **optional**, but they give you type safety, IDE refactor support, and protection from stringly-typed bugs.  
You *can* just use strings, but the keys are there for your own safety.



