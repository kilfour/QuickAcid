
using QuickAcid.Reporting;

namespace QuickAcid.Fluent;

public class Wendy : Bob<Acid>
{
    public Wendy(QAcidRunner<Acid> runner)
        : base(runner) { }

    public QAcidReport CheckForGold(int runs, int actions)
    {
        for (int i = 0; i < runs; i++)
        {
            var state = new QAcidState(runner);
            state.Run(actions);
            if (state.Failed)
                return state.GetReport();
        }
        return null!;
    }
}

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