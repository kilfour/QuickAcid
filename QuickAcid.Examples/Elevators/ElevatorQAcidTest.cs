using QuickMGenerate;

namespace QuickAcid.Examples.Elevators;

public class ElevatorQAcidTest : QAcidLoggingFixture
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

    [Fact]
    public void Minimal()
    {
        var run =
            from elevator in "Elevator".TrackedInput(() => new object())
            from _ in "nothing".Act(() => throw new Exception("boom"))
            from _s1 in "Always Execute and tru²e".SpecIf(() => true, () => true)
            from _s2 in MinimalSpec(elevator)
            select Acid.Test;
        var report = run.ReportIfFailed(30, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }

    private static QAcidRunner<Acid> MinimalSpec(object elevator)
    {
        return from _ in "minimalSpec".Spec(() => true) select Acid.Test;
    }

    [Fact]
    public void ElevatorRequestSystemWorks()
    {
        var run =
            from elevator in "Elevator".TrackedInput(() => new Elevator())
            from tracker in "Tracker".TrackedInput(() => new Tracker(elevator))
            from _ in "ops".Choose(
                MoveUpWithBounds(elevator, tracker),
                MoveDown(elevator, tracker),
                OpenDoors(elevator),
                CloseDoors(elevator),
                CallFloor(elevator, tracker),
                Step(elevator)
            ).Before(() => tracker.Do(elevator))
            from _s1 in "Initial floor is zero"
                .SpecIf(() => tracker.OperationsPerformed == 0, () => elevator.CurrentFloor == 0)
            from _s2 in AllRequestsServed(tracker)
            select Acid.Test;

        var report = run.ReportIfFailed(30, 10);
        if (report != null)
            Assert.Fail(report.ToString());

        // from _ in "All requests eventually served".Spec(
        // () =>
        //    {
        //        QAcidDebug.WriteLine($"tracker == null: {tracker == null}.");
        //        QAcidDebug.WriteLine($"CurrentFloor: {tracker.CurrentFloor}, DoorsOpen: {tracker.DoorsOpen}, Requests: [{string.Join(",", tracker.Requests)}]");
        //        try
        //        {
        //            return tracker.Requests.All(
        //                r => tracker.ServedRequests.Contains(r));
        //        }
        //        catch (Exception ex)
        //        {
        //            QAcidDebug.WriteLine("Spec crashed: " + ex);
        //            throw;
        //        }
        //    })
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

    private static QAcidRunner<Acid> CallFloor(Elevator elevator, Tracker tracker)
    {
        return
            from floor in "callFloor".Input(MGen.Int(0, Elevator.MaxFloor))
            from _a in "Call".Act(() =>
            {
                elevator.Call(floor);
                tracker.Requests.Add(floor); // for later specs
            })
            select Acid.Test;
    }

    private static QAcidRunner<Acid> Step(Elevator elevator)
    {
        return from _ in "Step".Act(elevator.Step) select Acid.Test;
    }

    private static QAcidRunner<Acid> AllRequestsServed(Tracker tracker)
    {
        return
            from _ in "All requests eventually served".Spec(
            () =>
               {
                   try
                   {
                       return tracker.Requests.All(
                           r => tracker.ServedRequests.Contains(r));
                   }
                   catch (Exception ex)
                   {
                       QAcidDebug.WriteLine("Spec crashed: " + ex);
                       throw;
                   }
               })
            select Acid.Test;
    }
}

