# QuickAcid

QuickAcid is a property-based testing library for C# that combines:

* LINQ-based test scripting
* Shrinkable, structured inputs
* Minimal-case failure reporting
* Customizable shrinking strategies
* Deep state modeling and execution traces

It’s designed for sharp diagnostics, elegant expressiveness, and easy extension.

## Example

Given a naive `Account` model:

```csharp
public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) => Balance += amount;
    public void Withdraw(int amount) => Balance -= amount;
}
```

You can test the overdraft invariant like this:

```csharp
var script =
    from account in "Account".Tracked(() => new Account(), a => a.Balance.ToString())
    from _ in "ops".Choose(
        from amount in "deposit".Input(Fuzz.Int(0, 10))
        from act in "account.Deposit".Act(() => account.Deposit(amount))
        select Acid.Test,
        from amount in "withdraw".Input(Fuzz.Constant(42))
        from act in "account.Withdraw".Act(() => account.Withdraw(amount))
        select Acid.Test)
    from spec in "No_Overdraft".Spec(() => account.Balance >= 0)
    select Acid.Test;

QState.Run(script)
    .WithOneRun()
    .And(50.ExecutionsPerRun());
```

## Example Failure Output

```
 ----------------------------------------
 -- Property 'No_Overdraft' was falsified
 -- Original failing run: 3 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (3 shrinks)
 -- Seed: 1254808606
 ----------------------------------------
 RUN START :
   => Account (tracked) : 0
 ---------------------------
 EXECUTE : account.Withdraw
   - Input : withdraw = 42
 ***************************
  Spec Failed : No_Overdraft
 ***************************
```


