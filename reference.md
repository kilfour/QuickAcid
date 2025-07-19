# QuickAcid
Before we get in the technical details and/or sales pitch, 
I feel it might help clear up what this thing actually does, and why it's useful, if I start with an example.  

So here goes :  

Given this naive, flawed, model:
```csharp
public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) { Balance += amount; }
    public void Withdraw(int amount) { Balance -= amount; }
}
```
And assuming that there exists a specification that an account holder should not be able to withdraw 
funds that will cause the account balance to go below zero, we can write the following QuickAcid test.
```csharp
var script =
    from account in "Account".Tracked(() => new Account(), a => a.Balance.ToString())
    from _ in "ops".Choose(
        from depositAmount in "deposit".Input(Fuzz.Int(0, 10))
        from act in "account.Deposit".Act(() => account.Deposit(depositAmount))
        select Acid.Test,
        from withdrawAmount in "withdraw".Input(Fuzz.Int(42, 42))
        from withdraw in "account.Withdraw:withdraw".Act(() => account.Withdraw(withdrawAmount))
        select Acid.Test
    )
    from spec in "No_Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
    select Acid.Test;
```
Running this test will produce the following output:
```
QuickAcid.Bolts.FalsifiableException : QuickAcid Report:
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
 Now you might say: "But I can easily write a unit test for that, ... like so:"
 ```csharp
[Fact]
public void Unit_test()
{
    var account = new Account();
    account.Withdraw(42);
    Assert.True(account.Balance >= 0);
}
```
Much simpler. Which is very true, and inadvertently, illustrates the point that property based testing is not meant to replace unit testing, but rather complement it.  
You see, if you look closer at the report you will notice that the QuickAcid test tried three operations and furthermore 
the number 42 you see in the output is not visible in the test. 
Yet it still managed to pinpoint the minimal failing case in order to fail the constraint we put on our model.  
So yes, if faced with this exact problem, write a unit test, but most problems aren't this obvious.  
And in order to track those down you might think about calling in QuickAcid.

*Would you like to know more ?*


## QuickAcid Linq 101
First of all, you need to import some namespaces if you want to use the Linq interface.  
This is intentional. The Linq interface assumes familiarity with either Linq combinators or property-based testing concepts.

```csharp
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
```

Yes, you need both the Nuts and the Bolts.


### What is a Script?
Scripts are the core abstraction of QuickAcid's LINQ model. 
They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

A script is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
It encapsulates both what to do and how to do it, with full access to the current test state.
```csharp
public delegate QAcidResult<T> QAcidScript<T>(QAcidState state);
```


You can think of scripts as the building blocks of a property-based test.


Each LINQ combinator constructs a new script by composing existing ones. 
The final test, the full LINQ query, is just a single, composed script.
The following is a (meaningless) example, demonstrating syntax:
```csharp
from spec in "spec".Spec(() => true)
select Acid.Test;
```
*Sidenote:* `.Spec(...)` makes an assertion. It checks whether a condition holds, and can pass or fail.
Will be explained in detail later.


### Acid.Test Explained

In QuickAcid, every test definition ends with:
```csharp
select Acid.Test;
```
`Acid.Test` is a unit value. It represents the fact that there is no meaningful return value to capture,
and serves as a **terminator** for the test chain.
*All well-formed tests should end with it.*

### What is a Run?
A **Run** in QuickAcid is a single attempt to validate a property.
It consists of one or more executions of the test logic, usually with different fuzzed inputs.
A run ends either when a failure is found or when the maximum number of executions is reached.
In order to turn a previously defined test into a run use the following pattern:
```csharp
var script =
    from spec in "spec".Spec(() => true)
    select Acid.Test;
new QState(script).Testify(1);
```
In the above example the variable 'run' is the definition, and the .Testify(1) call performs an instance of it once.
Seeing as the `Spec` (thoroughly explained later on) is hardcoded to pass,
nothing is reported and using this in a unit testing framework just passes the encompassing test. 


The int parameter passed in to the `Testify` method specifies number of executions per run.
An execution is one walk through the test definition.
Why we would want to do that multiple times will quickly become clear when we introduce other QAcidScripts.
For now, calling `.Testify(10)` in above example checks the Spec ten times.

By changing the return value of the `Spec` to `false` we can force this test to fail and in that case
a QuickAcid FalsifiableException is thrown containing a report.  
In this case :
```csharp
 ----------------------------------------
 -- Property 'spec' was falsified
 -- Original failing run: 1 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (2 shrinks)
 ----------------------------------------
 RUN START :
 *******************
  Spec Failed : spec
 *******************
```


### What is an Execution?
In the previous section we briefly mentioned executions, let's elaborate and have a look at a simple test:
 ```csharp
var script =
    from container in "container".Stashed(() => new Container(0))
    from input in "input".Input(Fuzz.Int(1, 5))
    from act in "act".Act(() => container.Value = input)
    from spec in "spec".Spec(() => container.Value != 0)
    select Acid.Test;

new QState(script).Testify(10);
```
While contrived, this example demonstrates how `Stashed`, `Input`, and `Act` work together across multiple executions.
First a brief explanation of the newly introduced Scripts :
- `Stashed(...)` — defines a named value that will be accessible during the test.
- `Input(...)` — introduces a fuzzed input that will be tracked and shrunk in case of failure.
- `Act(...)` — performs an action. It's where you apply behavior, such as calling a method or mutating state.

Suppose we execute this script with `new QState(script).Testify(10);`. What happens?  
As stated before, it will run 10 executions, but the individual scripts can have different scopes,
which is how QuickAcid handles mutable state and side effects.  

**Note on Scoping:**
`Stashed(...)` values are shared across executions, they persist for the entire run.
`Input(...)` values, on the other hand, are regenerated with each execution and shrink independently if a failure is detected.

**First execution** :
1. () => new Container() gets called and the result is stored in memory.
2. A 'shrinkable input' is generated using QuickFuzzr and stored in memory. Let's assume it returns 3. 
3. The act is performed and container.Value changed.
4. The invariant defined in spec is checked, and in this case (3 != 0) will pass.

**Consecutive executions** :
1. Because of `Stashed`, the value from memory for container is retrieved from the previous execution.
2. A new input is generated, let's assume 2.
3. The act is performed and container.Value changed.
4. The invariant is checked again (2 != 0), and again will pass.

If any execution fails, QuickAcid immediately halts the run and begins shrinking the input to a simpler failing case. A feature we will explore in detail later on. 


## QState
*Pronounced as:* Cue State.  

Which is exactly what it is for. The Linq Run definition is a stateless function.  
It defines the shape of computation and is fully self contained. But without state it is pretty much useless.


In order to turn it into something real we need to use QState, ... like so :
```csharp
new QState("act".Act(() => { /* nothing happening at all */ })).Testify(10);
```
What this does is, it takes a script (in this case: `.Act(...)`) and a state, in order to create a run and then performs said run consisting of 10 executions. 


In many examples here, you will see the following pattern :
```csharp
5.Times(() => new QState("act".Act(() => { /* nothing happening at all */ })).Testify(10));
```
`.Times()` is just some syntactic sugar on top of `for(var i = ... )`, so now the above example performs 5 runs of 10 executions each.  

*Why would you want to do that ?*  
Well performance reasons mainly, to help the shrinker. It is a lot easier to shrink 1 out of 50 runs of 100 executions than to shrink 1 run with 5000 executions.  

What number to plug in where depends entirely on what you are testing.  

**Note:** If this is such a common pattern, why not bake it into the library ?  
This was originally the case, but it was confusing even for me and i wrote the darn thing.  
So a decision was made to sacrifice a little bit of brevity in order to gain a lot of clarity.  


## QuickAcid Combinators
Combinators are the heart of QuickAcid's LINQ-based property testing DSL.  
Each one introduces a **step** in the test pipeline — a value, an action, a condition, or a check — and together they form the skeleton of your test logic.

Most combinators follow a simple pattern: 
they attach behavior to a named step (via `"name".Combinator(...)`) and compose naturally using LINQ syntax.

While each combinator has its own lifecycle and semantics, they all share one goal:  
to **express your intent declaratively** and let the QuickAcid engine take care of execution, shrinking, and reporting.

The sections below describe these core building blocks.



### Stashed
**Stashed(...)** creates a value once at the start of the test run and reuses it across all executions.  
This is typically where you'd stash your **system under test (SUT)** — a service, container, or domain object whose behavior you're exploring.  
Unlike `Input(...)`, stashed values are fixed for the entire run and never shrink, making them ideal for holding mutable state or orchestrating effects across inputs.


**Usage example:**
```csharp
from account in "account".Stashed(() => new Account())
```


##### StashedValue
Stashes a primitive or scalar value without requiring a wrapper object.
Intended for flags, counters, and other small mutable state used during generation.  

**Example:**
```csharp
from flag in "flag".StashedValue(true)
```


### Tracked
**Tracked(...)** behaves exactly like `Stashed(...)`: it defines a named value that is created once at the start of the test run and shared across all executions.
The key difference is that `Tracked(...) `also ensures this value is **always included in the final report**, providing visibility into contextual or setup state even when it wasn't directly responsible for the failure.


**Usage example:**
```csharp
from account in "account".Tracked(() => new Account(), a => a.Balance.ToString())
```
The second argument is a formatter function for rendering the value into the test report.


##### TrackedValue

Similar to `StashedValue(...)`, but again, this one shows up in the report.

**Example:**
```csharp
from flag in "flag".TrackedValue(true)
```


### Input
**Input(...)** introduces a fuzzed value that will be regenerated for every execution and shrunk when a failure occurs.  
It represents the core mechanism for exploring input space in property-based tests.  
Use it when you want a variable value that's subject to shrinking and included in the final report upon failure.

This is the most common kind of test input — think of it as the default for fuzzable values.

**Note:** If an input is not involved in a failing execution, it will not appear in the report.


**Usage example:**
```csharp
from input in "input".Input(() => Fuzz.Int())
```


### Act
**Act(...)** is your go to when you want to mutate your system under test.
####TODO: elaborate


**Usage example:**
```csharp
from act in "act".Act(() => account.Withdraw(500))
```



An overload of this combinator exists which can return a value, and therefor pass it down the LINQ chain.
```csharp
from act in "act".Act(() => account.GetBalance())
```


**Mutiple acts in one execution => can't shrink ! not the way to model things**
```csharp
from act1 in "act once".Act(() => account.Withdraw(500))
from act2 in "and act again".Act(() => account.Withdraw(200))
```


### Spec
**Spec(...)** ... TODO ...


**Usage example:**
```csharp
from specResult in "spec".Spec(() => false)
```


### DelayedSpec
**DelayedSpec(...)** ... TODO ...


**Usage example:**
```csharp
from spec in "spec".DelayedSpec(() => false)
from trace in "trace".TraceIf(() => spec.Failed, spec.Label)
let apply = spec.Apply()
```


### Choose
**Choose(...)** is one of two ways one can select different operations per execution.
In the case of choose, you supply a number of `.Act(...)`'s and QuickAcid will randomly choose one for every execution. 


**Usage example:**
```csharp
from _ in "ops".Choose(
    "deposit".Act(() => account.Deposit(500)),
    "withdraw".Act(() => account.Withdraw(500))
    )
```


### Derived
**Derived(...)** introduces a value that is dynamically generated during each execution, 
but is **not** shrinkable or tracked in the final report.  
Use this when you need to **react to mutable test state**, 
for example, selecting an input based on a previously `Stashed(...)` value.  

This is a niche combinator, 
primarily intended for state-sensitive generation where traditional shrinking would be inappropriate or misleading.



**Usage example:**
```csharp
from container in "container".Stashed(() => new Container<List<int>>([]))
from input in "input".Derived(Fuzz.ChooseFromWithDefaultWhenEmpty(container.Value))
```



### TestifyProvenWhen
**TestifyProvenWhen(...)**
Ends the test run early once a specified condition is satisfied.
This combinator is not a property specification itself,
but a control structure that governs when a test run is considered 'proven' and can terminate before reaching the maximum number of executions. It's typically used in combination with `Stashed(...)` or other state-tracking steps that accumulate evidence across runs.


**Usage example:**
```csharp
from seenTrue in "val is true".TestifyProvenWhen(() => container.Value)
```



This would end the test run early once `container.Value` becomes `true`.


**Note:** This does not assert a property directly — use `Assay(...)` or `Analyze(...)` for that.
`TestifyProvenWhen(...)` is about controlling *how long* a test runs based on dynamic conditions observed during execution.


### Trace
**Trace(...)** Used to add information from the script to the QuickAcid report.


**Usage example:**
```csharp
from _ in "Info Key".Trace(() => "Extra words")
```


**TraceIf(...)** is the same as `Trace(...)` but only injects information in the report conditionally.  

**Usage example:**
```csharp
from _ in "Info Key".TraceIf(() => number == 42, () => "Extra words")
```


### Skip
**Skip(...)** extension method for `QAcidScript<T>`. Useful for temporary disabling `Spec`'s.


**Usage example:**
```csharp
from __ in "spec".Spec(() => false).Skip()
```


**SkipIf(...)** is the same as `Skip(...)` but only skips conditionally.  

**Usage example:**
```csharp
from __ in "spec".Spec(() => false).SkipIf(() => true)
```


### Primitives Shrinking
`PrimitiveShrinkStrategy` is a shrinker used in QuickAcid to simplify failing test inputs for known primitive types.
It operates using a predefined list of alternative values per type and attempts to
replace the original with a simpler version that still causes the test to fail.


#### How It Works
1. **Recognize Known Type**  
   Checks if the given value belongs to a supported primitive type (like `int`, `bool`, `string`, etc.).
2. **Initial Check**  
   First ensures that the original value *actually* causes a failure. Otherwise, shrinking is skipped.
3. **Candidate Evaluation**
   - Iterates over alternative values (excluding the current one).
   - Tries all candidates. If one causes the test to pass, the current value is minimal.
4. **Trace Result**  
   After evaluation, it emits a trace indicating whether the value was kept, or marked irrelevant.


#### Supported Types
These are matched via `Type.IsAssignableFrom(...)` to allow some flexibility:

- **Boolean**: `true`, `false`
- **Numeric Types**: `int`, `long`, `short`, `byte`, `float`, `double`, `decimal`, including unsigned variants
- **Characters**: `\0`, `'a'`, `'Z'`, space, newline, `\uFFFF`
- **Strings**: `null`, empty, short strings, long strings
- **Time Types**: `DateTime`, `DateTimeOffset`, `TimeSpan`
- **Miscellaneous**: `Guid`, `Uri`


### Collection Shrinking
...


Usage

### Custom Shrinking
...


Usage with class

Usage with lambda

custom list strat

Custom Collection Shrinking Policy: RemoveOneByOneStrategy

ShrinkEachElementStrategy

## QuickAcid Logging
Let's not call a spade a shovel: property-based testing (PBT) isn't the easiest thing in the world.
I usually frown upon verbosity and an overdose of logging, but here, it's not just tolerable, it's necessary.
Especially when the user starts to dig a little deeper and implements its own custom shrinkers for instance.  

There are two ways to get diagnostics out of QuickAcid's engine.


### Verbose Mode
By adding a call to `.Verbose()` when building your test, you instruct the engine to include detailed diagnostic output in the report.
```csharp
var script =
    from spec in "spec".Spec(() => false)
    select Acid.Test;
new QState(script).Verbose().Testify(1);
```
This will produce a report that contains :


 - Information about the first failed run.

 - Information about the run after execution shrinking.


 - Information about the run after action shrinking.


 - Information about the run after input shrinking.


Which for this example: 
```csharp
var script =
    from container in "stashed".Stashed(() => new Container(0))
    from input in "input".Input(Fuzz.Int(1, 6))
    from act in "act".Act(() => container.Value = input)
    from spec in "spec".Spec(() => container.Value != 5)
    select Acid.Test;
new QState(script).Verbose().Testify(50);
```
Outputs something similar to:
```
 ----------------------------------------
 -- FIRST FAILED RUN
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : act
   - Input : input = 2
 ---------------------------
 EXECUTE : act
   - Input : input = 2
 ---------------------------
 EXECUTE : act
   - Input : input = 3
 ---------------------------
 EXECUTE : act
   - Input : input = 2
 ---------------------------
 EXECUTE : act
   - Input : input = 5
 *******************
  Spec Failed : spec
 *******************

 ----------------------------------------
 -- AFTER EXECUTION SHRINKING
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : act
   - Input : input = 5
 *******************
  Spec Failed : spec
 *******************

 ----------------------------------------
 -- AFTER INPUT SHRINKING :
 -- Property 'spec' was falsified
 -- Original failing run: 5 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (6 shrinks)
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : act
   - Input : input = 5
 *******************
  Spec Failed : spec
 *******************
 ```


