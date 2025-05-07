using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.CueState;

public static class Chapter { public const string Order = "1-20"; }

[Doc(Order = Chapter.Order, Caption = "QState", Content =
@"Pronounced as : Cue State.  
Which is exactly what it is for. The Linq Run definition is a stateless function.  
It defines the shape of computation and is fully self contained. But without state it is pretty much useless.
In order to turn it into something real we can use QState.
")]
public class QStateTests
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1", Content = "...")]
    public void What_is_a_single_runner()
    {
        Assert.IsType<QAcidRunner<int>>("an int".Input(MGen.Int()));
    }
}