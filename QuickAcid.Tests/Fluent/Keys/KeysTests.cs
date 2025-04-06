using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Perform;

public class KeysTests
{
    [Fact]
    public void KeySpike()
    {
        SystemSpecs.Define()
            .TrackedInput(QKey<int>.New("answer"), () => 42)
                .Perform("1", () => { })
                .DumpItInAcid()
                .AndCheckForGold(1, 20);
    }
}

// internal class Lofty<T>
// {
//     private readonly Bob<T> parent;
//     private readonly string label;
//     private readonly Action action;

//     public Lofty(Bob<T> parent, string label, Action action)
//     {
//         this.parent = parent;
//         this.label = label;
//         this.action = action;
//     }

//     public Bob<T> If(Func<QAcidContext, bool> condition)
//     {
//         return parent.BindState(state =>
//             condition(state)
//                 ? label.Act(action)
//                 : QAcidRunners.NoOp(label));
//     }

//     public Bob<T> If(Func<bool> condition)
//         => If(_ => condition());
// }
