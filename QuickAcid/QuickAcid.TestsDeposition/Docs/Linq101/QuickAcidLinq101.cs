using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Docs.Linq101;

public static class Chapter { public const string Order = "1-1"; }

[Doc(Order = Chapter.Order, Caption = "QuickAcid Linq 101", Content =
@"First of all, you need to import some namespaces if you want to use the Linq interface.  
This is intentional. The Linq interface assumes familiarity with either Linq combinators or property-based testing concepts.

```csharp
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
```

Yes, you need both the Nuts and the Bolts.
")]
[Doc(Order = $"{Chapter.Order}-1", Caption = "What is a Script?", Content =
@"Scripts are the core abstraction of QuickAcid's LINQ model. 
They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

A script is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
It encapsulates both what to do and how to do it, with full access to the current test state.
```csharp
public delegate QAcidResult<T> QAcidScript<T>(QAcidState state);
```
")]
public class QuickAcidLinq101
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1-1", Content = "You can think of scripts as the building blocks of a property-based test.")]
    public void What_is_a_single_script()
    {
        Assert.IsType<QAcidScript<int>>("an int".Input(MGen.Int()));
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-1-2", Content =
@"
Each LINQ combinator constructs a new script by composing existing ones. 
The final test, the full LINQ query, is just a single, composed script.
The following is a (meaningless) example, demonstrating syntax:
```csharp
from spec in ""spec"".Spec(() => true)
select Acid.Test;
```
*Sidenote:* `.Spec(...)` makes an assertion. It checks whether a condition holds, and can pass or fail.
Will be explained in detail later.
")]
    public void What_is_a_composed_script()
    {
        Assert.IsType<QAcidScript<Acid>>(
            from spec in "spec".Spec(() => true)
            select Acid.Test);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-2", Caption = "Acid.Test Explained", Content =
@"
In QuickAcid, every test definition ends with:
```csharp
select Acid.Test;
```
`Acid.Test` is a unit value. It represents the fact that there is no meaningful return value to capture,
and serves as a **terminator** for the test chain.
*All well-formed tests should end with it.*")]
    public void What_is_Acid_test()
    {
        Assert.IsType<Acid>(Acid.Test);
        Assert.IsType<QAcidScript<Acid>>(from input in "act".Act(() => { }) select Acid.Test);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-3", Caption = "What is a Run?", Content =
@"A **Run** in QuickAcid is a single attempt to validate a property.
It consists of one or more executions of the test logic, usually with different fuzzed inputs.
A run ends either when a failure is found or when the maximum number of executions is reached.
In order to turn a previously defined test into a run use the following pattern:
```csharp
var run =
    from spec in ""spec"".Spec(() => true)
    select Acid.Test;
new QState(run).Testify(1);
```
In the above example the variable 'run' is the definition, and the .Testify(1) call performs an instance of it once.
Seeing as the `Spec` (thoroughly explained later on) is hardcoded to pass,
nothing is reported and using this in a unit testing framework just passes the encompassing test. 
")]
    public void What_is_a_run()
    {
        var timesSpecChecked = 0;
        var run =
            from spec in "spec".Spec(() => { timesSpecChecked++; return true; })
            select Acid.Test;
        var ex = Record.Exception(() => new QState(run).Testify(1));
        Assert.Null(ex);
        Assert.Equal(1, timesSpecChecked);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-3-2", Content =
@"The int parameter passed in to the `Testify` method specifies number of executions per run.
An execution is one walk through the test definition.
Why we would want to do that multiple times will quickly become clear when we introduce other QAcidScripts.
For now, calling `.Testify(10)` in above example checks the Spec ten times.")]
    public void What_is_a_run_multiple_executions()
    {
        var timesSpecChecked = 0;
        var run =
            from spec in "spec".Spec(() => { timesSpecChecked++; return true; })
            select Acid.Test;
        var ex = Record.Exception(() => new QState(run).Testify(10));
        Assert.Null(ex);
        Assert.Equal(10, timesSpecChecked);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-3-3", Content =
@"By changing the return value of the `Spec` to `false` we can force this test to fail and in that case
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
")]
    public void What_is_a_run_failing_spec_throws()
    {
        var run =
            from spec in "spec".Spec(() => false)
            select Acid.Test;

        var ex = Assert.Throws<FalsifiableException>(() => new QState(run).Testify(1));
        Assert.NotNull(ex.QAcidReport);
    }


    [Fact]
    [Doc(Order = $"{Chapter.Order}-4", Caption = "What is an Execution?", Content =
@"In the previous section we briefly mentioned executions, let's elaborate and have a look at a simple test:
 ```csharp
var run =
    from container in ""container"".Stashed(() => new Container(0))
    from input in ""input"".Input(MGen.Int(1, 5))
    from act in ""act"".Act(() => container.Value = input)
    from spec in ""spec"".Spec(() => container.Value != 0)
    select Acid.Test;

new QState(run).Testify(10);
```
While contrived, this example demonstrates how `Stashed`, `Input`, and `Act` work together across multiple executions.
First a brief explanation of the newly introduced Scripts :
- `Stashed(...)` — defines a named value that will be accessible during the test.
- `Input(...)` — introduces a fuzzed input that will be tracked and shrunk in case of failure.
- `Act(...)` — performs an action. It's where you apply behavior, such as calling a method or mutating state.

Suppose we execute this script with `new QState(run).Testify(10);`. What happens?  
As stated before, it will run 10 executions, but the individual scripts can have different scopes,
which is how QuickAcid handles mutable state and side effects.  

**Note on Scoping:**
`Stashed(...)` values are shared across executions, they persist for the entire run.
`Input(...)` values, on the other hand, are regenerated with each execution and shrink independently if a failure is detected.

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
")]
    public void What_is_an_execution()
    {
        var run =
            from container in "container".Stashed(() => new Container<int>(0))
            from input in "input".Input(MGen.Int(1, 5))
            from act in "act".Act(() => container.Value = input)
            from spec in "spec".Spec(() => container.Value != 0)
            select Acid.Test;
        new QState(run).Testify(10);
    }
}