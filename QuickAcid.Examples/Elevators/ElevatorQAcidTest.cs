using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Examples.Elevators;

public class ElevatorQAcidTest : QAcidLoggingFixture
{

    [Fact(Skip = "demo")]
    public void ElevatorRequestSystemWorks()
    {
        var report = TheRun.ReportIfFailed(30, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }

    private static QAcidRunner<Acid> TheRun =
        from elevator in "Elevator".AlwaysReported(() => new Elevator())
        from tracker in "Tracker".AlwaysReported(() => new Tracker(elevator))
        from _ in "ops".Choose(
            MoveUp(elevator, tracker),
            MoveDown(elevator, tracker),
            OpenDoors(elevator),
            CloseDoors(elevator)//,
            // CallFloor(elevator, tracker),
            // Step(elevator)
        ).Before(() => tracker.Do(elevator))
        from _s1 in "Initial floor is zero"
            .SpecIf(() => tracker.OperationsPerformed == 0, () => elevator.CurrentFloor == 0)
        from _s2 in AllRequestsServed(tracker)
        select Acid.Test;

    // this creates a type initializer problemm
    private static Func<QAcidRunner<Elevator>> ElevatorInput =
        () => "Elevator".AlwaysReported(() => new Elevator());

    private static QAcidRunner<Acid> MoveUp(Elevator elevator, Tracker tracker)
    {
        return
            from _ in "MoveUp".Act(elevator.MoveUp)
            from _s1 in "MoveUp Does Not Exceed Max Floor"
                .Spec(() => elevator.CurrentFloor <= Elevator.MaxFloor)
            from _s2 in "MoveUp Increments Floor When Doors Are Closed and Below Max"
                .SpecIf(
                    () => !tracker.DoorsOpen && tracker.CurrentFloor < Elevator.MaxFloor,
                    () => elevator.CurrentFloor == tracker.CurrentFloor + 1)
            from _s3 in "MoveUp Does Not Increment When At Max Floor"
                .SpecIf(
                    () => tracker.CurrentFloor >= Elevator.MaxFloor,
                    () => elevator.CurrentFloor == tracker.CurrentFloor)
            select Acid.Test;
    }

    private static QAcidRunner<Acid> MoveDown(Elevator elevator, Tracker tracker)
    {
        return
            from __a in "MoveDown".Act(elevator.MoveDown)
                // from _s1 in "MoveDown Decrements Floor When Doors Are Closed"
                //     .SpecIf(
                //         () => !tracker.DoorsOpen,
                //         () => elevator.CurrentFloor == tracker.CurrentFloor - 1)
            from _s2 in "MoveDown Does Not Decrement When Doors Are Open"
                .SpecIf(
                    () => tracker.DoorsOpen,
                    () => elevator.CurrentFloor == tracker.CurrentFloor)
            from _s3 in "MoveDown Does Not Decrement When At Ground Floor"
                .SpecIf(
                    () => tracker.CurrentFloor == 0,
                    () => elevator.CurrentFloor == tracker.CurrentFloor)
            select Acid.Test;
    }

    private static QAcidRunner<Acid> OpenDoors(Elevator elevator) =>
        "OpenDoors".Act(elevator.OpenDoors);

    private static QAcidRunner<Acid> CloseDoors(Elevator elevator) =>
        "CloseDoors".Act(elevator.CloseDoors);

    private static QAcidRunner<Acid> CallFloor(Elevator elevator, Tracker tracker)
    {
        return
            from floor in "callFloor".Input(MGen.Int(0, Elevator.MaxFloor))
            from _a in "Call".Act(() =>
            {
                elevator.Call(floor);
                tracker.Requests.Add(floor);
            })
            select Acid.Test;
    }

    private static QAcidRunner<Acid> Step(Elevator elevator)
        => "Step".Act(elevator.Step);

    private static QAcidRunner<Acid> AllRequestsServed(Tracker tracker) =>
        "All requests eventually served".Spec(
            () => tracker.Requests.All(r => tracker.ServedRequests.Contains(r)));
}

