# QuickAcid.TheFortyNiners


`QuickAcid.TheFortyNiners` is a lightweight starting point for property-based testing (PBT) in C#, powered by [QuickAcid](https://github.com/kilfour/QuickAcid) under the hood.

With it you can **travel light**, **strike gold fast**, and **dig deeper later** when you're ready.

- **Simple, clean syntax** — familiar to unit testing fans
- **Full property-based power** under the surface
- **No need to learn everything up front**

---

## Quick Example: A Unit Test

```csharp
using QuickAcid.TheFortyNiners;

[Fact]
public void Account_starts_positive()
{
    Test.This(() => new Account(), a => a.Balance.ToString())
        .Arrange(("withdraw", 20))
        .Act(Perform.This("withdraw", 
            (Account account, int withdraw) 
                => account.Withdraw(withdraw)))
        .Assert("No Overdraft", account => account.Balance >= 0)
        .UnitRun();
}
```

---

## Quick Example: Your First PBT

```csharp
using QuickAcid.TheFortyNiners;

[Fact]
public void Account_balance_stays_in_range()
{
    Test.This(() => new Account(), a => a.Balance.ToString())
        .Arrange(
            ("dep", MGen.Int(0, 100)),
            ("wd", MGen.Int(0, 100)))
        .Act(
            Perform.This("deposit", 
                (Account account, int dep) 
                    => account.Deposit(deposit)),
            Perform.This("withdraw", 
                (Account account, int wd) 
                    => account.Withdraw(withdraw)))
        .Assert("No Overdraft", account => account.Balance >= 0)
        .Assert("Balance Capped", account => account.Balance <= 100)
        .Run(1, 10);
}
```

---

## Why use TheFortyNiners?

- **Familiar structure** — Arrange, Act, Assert, just like unit tests.
- **Quick start** — Focus on writing meaningful tests, not wiring.
- **Property-based power** — Randomized operations, shrinking minimal examples, and bug discovery — all built-in.
- **Easy upgrade path** — When you're ready, move into the full fluent stack for complete control over fuzzing, tracking, shrinking, and custom behaviors.

---

## A Note About Namespaces

If you `using QuickAcid.TheFortyNiners;`, you're in light-mode:  
- Simple `Test.This(...).Arrange(...).Act(...).Assert(...)` flows.
- Great for everyday unit tests *and* lightweight property-based tests.

If you later switch to `using QuickAcid;`, you get full control over:
- Custom shrinkers
- Tracked state and inputs
- Conditional execution and guards
- Phase-specific specs
- Formal model-based exploration

The transition is smooth, no wasted work, just more options when you want them!

---


