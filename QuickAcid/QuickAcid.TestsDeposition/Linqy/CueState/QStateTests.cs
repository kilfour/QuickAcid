using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.CueState;

public static class Chapter { public const string Order = "1-2"; }

[Doc(Order = Chapter.Order, Caption = "QState", Content =
@"Pronounced as : Cue State.  
Which is exactly what it is for. The Linq Run definition is a stateless function.  
It defines the shape of computation and is fully self contained. But without state it is pretty much useless.
In order to turn it into something real we can use QState.
```csharp
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
```

Yes, you need both the Nuts and the Bolts.
")]
[Doc(Order = $"{Chapter.Order}-1", Caption = "What is a Runner?", Content =
@"Runners are the core abstraction of QuickAcid's LINQ model. 
They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

A runner is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
It encapsulates both what to do and how to do it, with full access to the current test state.
```csharp
public delegate QAcidResult<T> QAcidRunner<T>(QAcidState state);
```
")]
public class QStateTests
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1-1", Content = "You can think of runners as the building blocks of a property-based test.")]
    public void What_is_a_single_runner()
    {
        Assert.IsType<QAcidRunner<int>>("an int".Shrinkable(MGen.Int()));
    }
}