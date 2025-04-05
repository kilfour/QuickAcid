using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Perform;

public class OptionsTests
{
    [Fact]
    public void Spike()
    {

        var report =
           SystemSpecs.Define()
                .Options(opt => [
                    opt.Perform("1", () => { }),
                    opt.Perform("2", () => { }) ])
                .PickOne()
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
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

// ElevatorSystemSpec.Define()
//     .With(e => new Elevator())
//     .TrackStateWith(e => new Tracker(e))
//     .PerformOpsRandomly("ops", ops => ops.Choose(
//         Act("CallFloor", () => e.Call(RandomFloor())),
//         Act("Step", e.Step),
//         Act("OpenDoors", e.OpenDoors),
//         Act("CloseDoors", e.CloseDoors)
//     ))
//     .Spec("Doors only open when at requested floor")
//         .OnlyWhen(() => e.Requests.Contains(e.CurrentFloor))
//         .Assert(() => e.DoorsOpen)
//     .Spec("No floor is skipped")
//         .Eventually(() =>
//             tracker.Requests.All(f => tracker.Served.Contains(f)))
//     .Spec("Max floor not exceeded")
//         .Always(() => e.CurrentFloor <= Elevator.MaxFloor)
//     .Verify(100 runs, 10 steps);