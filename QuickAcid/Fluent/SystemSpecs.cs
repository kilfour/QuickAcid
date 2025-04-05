namespace QuickAcid.Fluent;
public static class SystemSpecs
{
    public static Bob<Acid> Define()
    {
        return new Bob<Acid>(state => new QAcidResult<Acid>(state, Acid.Test));
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