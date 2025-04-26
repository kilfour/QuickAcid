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

## Examples

```csharp
Test.This(() => new Account())
    .Arrange(
        ("deposit", MGen.Int(0, 100).AsObject()),
        ("withdraw", MGen.Int(0, 100).AsObject()))
    .Act(
        Perform.Action("deposit", (Account account, int deposit) => account.Deposit(deposit)),
        Perform.Action("withdraw", (Account account, int withdraw) => account.Withdraw(withdraw)))
    .Assert("No Overdraft", account => account.Balance >= 0)
    .Assert("Balance Capped", account => account.Balance <= 100)
    .Run(50, 10);
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



