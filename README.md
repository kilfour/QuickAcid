# QuickAcid
> Drop it in acid. Look for gold.  
> Like alchemy, but reproducible.
  
QuickAcid is a property-based testing library for C# that combines:

* LINQ-based test scripting
* Shrinkable, structured inputs
* Minimal-case failure reporting
* Customizable shrinking strategies
* Deep state modeling and execution traces

It's designed for sharp diagnostics, elegant expressiveness, and easy extension.  
## Example
Given a naive `Account` model:  
```csharp
public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) { Balance += amount; }
    public void Withdraw(int amount) { Balance -= amount; }
}
```
You can test the overdraft invariant like this:  
```csharp
var script =
    from account in Script.Tracked(() => new Account())
    from _ in Script.Choose(
        from amount in Script.Input<Deposit.Amount>().With(Fuzzr.Int(0, 10))
        from act in Script.Act<Deposit>(() => account.Deposit(amount))
        select Acid.Test,
        from amount in Script.Input<Withdraw.Amount>().With(Fuzzr.Int(0, 10))
        from act in Script.Act<Withdraw>(() => account.Withdraw(amount))
        select Acid.Test)
    from spec in Script.Spec<NoOverdraft>(() => account.Balance >= 0)
    select Acid.Test;
QState.Run(script, 1584314623) // <= reproducible when seeded
    .WithOneRun()
    .And(50.ExecutionsPerRun());
```
The generic arguments to the various `Script` methods are just lightweight marker records, used for labeling inputs, actions, and specifications in reports:  
```csharp
namespace QuickAcid.Tests;

public record Deposit : Act { public record Amount : Input; };
public record Withdraw : Act { public record Amount : Input; };
public record NoOverdraft : Spec;
```
## Example Failure Output
A failing property produces a minimal counterexample and a readable execution trace:  
```
──────────────────────────────────────────────────
 Test:                    ExampleTest
 Location:                C:\Code\QuickAcid\QuickAcid.Tests\CreateReadMe.cs:107:1
 Original failing run:    4 executions
 Minimal failing case:    1 execution (after 4 shrinks)
 Seed:                    1584314623
 ──────────────────────────────────────────────────
   => Account (tracked) : { Balance: 0 }
 ──────────────────────────────────────────────────
  Executed (3): Withdraw
   - Input: Withdraw Amount = 9
 ═══════════════════════════════
  ❌ Spec Failed: No overdraft
 ═══════════════════════════════
 Passed Specs
 - No overdraft: 3x
 ──────────────────────────────────────────────────
```
