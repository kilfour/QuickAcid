## QuickAcid Linq 101

First of all you are going to need import some namespaces if you want to use the Linq interface.  
This is by design. If you're using the Linq interface there is an underlying assumption you have some experience
either with Linq combinators or property based testing.
```csharp
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
```
Yes, you need both the Nuts and the Bolts.


### What is a Runner?

Runners are the core abstraction of QuickAcid's LINQ model. 
They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

A runner is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
It encapsulates both what to do and how to do it, with full access to the current test state.
```csharp
public delegate QAcidResult<T> QAcidRunner<T>(QAcidState state);
```


You can think of runners as the building blocks of a property-based test.


Each LINQ combinator constructs a new runner by composing existing ones. 
The final test, the full LINQ query, is just a single, composed runner.
The following is a (meaningless) example, demonstrating syntax:
```csharp
from spec in "spec".Spec(() => true)
select Acid.Test;
```
*Sidenote:* `.Spec(...)` makes an assertion. It checks whether a condition holds, and can pass or fail.
Will be explained in detail later.


---

### Acid.Test Explained


In QuickAcid, every test definition ends with:
```csharp
select Acid.Test;
```
`Acid.Test` is a unit value. It represents the fact that there is no meaningful return value to capture,
and serves as a **terminator** for the test chain.
*All well-formed tests should end with it.*

---

### What is a Run?

A **Run** in QuickAcid is a single attempt to validate a property.
It consists of one or more executions of the test logic, usually with different fuzzed inputs.
A run ends either when a failure is found or when the maximum number of executions is reached.
In order to turn a previously defined test into a run use the following pattern:
```csharp
var run =
    from spec in "spec".Spec(() => true)
    select Acid.Test;
new QState(run).Testify(1);
```
In the above example the variable 'run' is the definition, and the .Testify(1) call performs an instance of it once.
Seeing as the `Spec` (thoroughly explained later on) is hardcoded to pass,
nothing is reported and using this in a unit testing framework just passes the encompassing test. 


The int parameter passed in to the `Testify` method specifies number of executions per run.
An execution is one walk through the test definition.
Why we would want to do that multiple times will quickly become clear when we introduce other QAcidRunners.
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


---

### What is an Execution?

In the previous section we briefly mentioned executions, let's elaborate and have a look at a simple test:
 ```csharp
var run =
    from container in "container".Stashed(() => new Container())
    from input in "input".Shrinkable(MGen.Int(1, 5))
    from act in "act".Act(() => container.Value = input)
    from spec in "spec".Spec(() => container.Value != 0)
    select Acid.Test;

new QState(run).Testify(10);
```
This example isn't meaningful on its own, but it's designed to illustrate the concepts of Run, Execution, and Scoping.
First a brief explanation of the newly introduced Runners :
- `Stashed(...)` — defines a named value that will be accessible during the test.
- `Shrinkable(...)` — introduces a fuzzed input that will be tracked and shrunk in case of failure.
- `Act(...)` — performs an action. It's where you apply behavior, such as calling a method or mutating state.

Suppose we execute this runner with `new QState(run).Testify(10);`. What happens?  
As stated before, it will run 10 executions, but the individual runners can have different scopes,
which is how QuickAcid handles mutable state and side effects.  

**Note on Scoping:**
`Stashed(...)` values are shared across executions, they persist for the entire run.
`Shrinkable(...)` values, on the other hand, are regenerated with each execution and shrink independently if a failure is detected.

**First execution** :
1. () => new Container() gets called and the result is stored in memory.
2. A 'shrinkable input' is generated using QuickMGenerate and stored in memory. Let's assume it returns 3. 
3. The act is performed and container.Value changed.
4. The invariant defined in spec is checked, and in this case (3 != 0) will pass.

**Consecutive executions** :
1. Because of `Stashed`, the value from memory for container is retrieved from the previous execution.
2. A new input is generated, let's assume 2.
3. The act is performed and container.Value changed.
4. The invariant is checked again (2 != 0), and again will pass.

If any execution fails, QuickAcid immediately halts the run and begins shrinking the input to a simpler failing case. A feature we will explore in detail later on. 


---

## QuickAcid Logging

Let's not call a spade a shovel: property-based testing (PBT) isn't the easiest thing in the world.
I usually frown upon verbosity and an overdose of logging, but here, it's not just tolerable, it's necessary.
Especially when the user starts to dig a little deeper and implements its own custom shrinkers f.i.  

There are two ways to get diagnostics out of QuickAcid's engine.


### Verbose Mode

By adding a call to `.Verbose()` when building your test, you instruct the engine to include detailed diagnostic output in the report.
```csharp
var run =
    from spec in "spec".Spec(() => false)
    select Acid.Test;
new QState(run).Verbose().Testify(1);
```
This will produce a report that contains :


 - Information about the first failed run.

 - Information about the run after execution shrinking.


 - Information about the run after input shrinking.


Which for this example: 
```csharp
var run =
    from container in "stashed".Stashed(() => new Container())
    from input in "input".Shrinkable(MGen.Int(1, 6))
    from act in "act".Act(() => container.Value = input)
    from spec in "spec".Spec(() => container.Value != 5)
    select Acid.Test;
new QState(run).Verbose().Testify(50);
```
Ouputs something similar to:
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


