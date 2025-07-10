using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;

namespace QuickAcid.TestsDeposition.Docs.CueState;

public static class Chapter { public const string Order = "1-20"; }

[Doc(Order = Chapter.Order, Caption = "QState", Content =
@"*Pronounced as:* Cue State.  

Which is exactly what it is for. The Linq Run definition is a stateless function.  
It defines the shape of computation and is fully self contained. But without state it is pretty much useless.
")]
public class QStateTests
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"In order to turn it into something real we need to use QState, ... like so :
```csharp
new QState(""act"".Act(() => { /* nothing happening at all */ })).Testify(10);
```
What this does is, it takes a script (in this case: `.Act(...)`) and a state, in order to create a run and then performs said run consisting of 10 executions. 
")]
    public void QState_usage()
    {
        var counter = 0;
        new QState("act".Act(() => { counter++; })).Testify(10);
        Assert.Equal(10, counter);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-2", Content =
@"In many examples here, you will see the following pattern :
```csharp
5.Times(() => new QState(""act"".Act(() => { /* nothing happening at all */ })).Testify(10));
```
`.Times()` is just some syntactic sugar on top of `for(var i = ... )`, so now the above example performs 5 runs of 10 executions each.  

*Why would you want to do that ?*  
Well performance reasons mainly, to help the shrinker. It is a lot easier to shrink 1 out of 50 runs of 100 executions than to shrink 1 run with 5000 executions.  

What number to plug in where depends entirely on what you are testing.  

**Note:** If this is such a common pattern, why not bake it into the library ?  
This was originally the case, but it was confusing even for me and i wrote the darn thing.  
So a decision was made to sacrifice a little bit of brevity in order to gain a lot of clarity.  
")]
    public void QState_n_times()
    {
        var counter = 0;
        5.Times(() => new QState("act".Act(() => { counter++; })).Testify(10));
        Assert.Equal(50, counter);
    }
}