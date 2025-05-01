using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.Basics
{
    [Doc(Order = "1-1-1", Caption = "What is a Runner?", Content =
@"Runners are the core abstraction of QuickAcid's LINQ model. 
They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

A runner is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
It encapsulates both what to do and how to do it, with full access to the current test state.
```csharp
public delegate QAcidResult<T> QAcidRunner<T>(QAcidState state);
```
")]
    public class What_is_a_runner
    {
        [Fact]
        [Doc(Order = "1-1-1-1", Content = "You can think of runners as the building blocks of a property-based test.")]
        public void What_is_a_single_runner()
        {
            Assert.IsType<QAcidRunner<int>>("an int".Shrinkable(MGen.Int()));
        }

        [Fact]
        [Doc(Order = "1-1-1-2", Content =
@"
Each LINQ combinator constructs a new runner by composing existing ones. 
The final test, the full LINQ query, is just a single, composed runner.
")]
        public void What_is_a_composed_runner()
        {
            Assert.IsType<QAcidRunner<Acid>>(
                from input in "act".Act(() => { })
                from spec in "spec.".Spec(() => true)
                select Acid.Test);
        }

        [Fact]
        [Doc(Order = "1-1-1-3", Caption = "`Acid.Test`", Content =
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
            Assert.IsType<QAcidRunner<Acid>>(from input in "act".Act(() => { }) select Acid.Test);
        }
    }
}