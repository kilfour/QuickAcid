namespace QuickAcid.Examples.Elevators;

public class ElevatorQAcidTest
{
    [Fact]
    public void RunAndVerify()
    {
        var run =
            from elevator in "Elevator".TrackedInput(
                () => new Elevator(),
                a => $"CurrentFloor: {a.CurrentFloor}, DoorsOpen: {a.DoorsOpen}")
            from tracker in "Tracker".TrackedInput(
                () => new Tracker(elevator),
                a => $"CurrentFloor: {a.CurrentFloor}, DoorsOpen: {a.DoorsOpen}")
            from _o in "ops".Choose(
                MoveUpWithBounds(elevator, tracker),
                MoveDown(elevator, tracker),
                OpenDoors(elevator),
                CloseDoors(elevator))
            .Before(() => tracker.Do(elevator))
            from _s in "Initial floor is zero"
                .SpecIf(
                    () => tracker.OperationsPerformed == 0,
                    () => elevator.CurrentFloor == 0)
            select Acid.Test;

        var report = run.ReportIfFailed(20, 5);
        if (report != null)
            Assert.Fail(report.ToString());
    }

    private static QAcidRunner<Acid> MoveUpWithBounds(Elevator elevator, Tracker tracker)
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
            from _s1 in "MoveDown Decrements Floor When Doors Are Closed"
                .SpecIf(
                    () => !tracker.DoorsOpen,
                    () => elevator.CurrentFloor == tracker.CurrentFloor - 1)
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

    private static QAcidRunner<Acid> OpenDoors(Elevator elevator)
    {
        return from _ in "OpenDoors".Act(elevator.OpenDoors) select Acid.Test;
    }

    private static QAcidRunner<Acid> CloseDoors(Elevator elevator)
    {
        return from _ in "CloseDoors".Act(elevator.CloseDoors) select Acid.Test;
    }
}
