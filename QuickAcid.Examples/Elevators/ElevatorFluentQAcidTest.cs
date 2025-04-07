using QuickAcid.Fluent;

namespace QuickAcid.Examples.Elevators;

public class ElevatorFluentQAcidTest : QAcidLoggingFixture
{
    public class TheHelp
    {
        public Elevator Elevator { get; set; }
        public Tracker Tracker { get; set; }

        public TheHelp(Elevator elevator)
        {
            Elevator = elevator;
            Tracker = new Tracker(elevator);
        }
    }

    public static class Keys
    {
        public static QKey<TheHelp> TheHelp =>
            QKey<TheHelp>.New("TheHelp");
    }

    // public class Context
    // {
    //     public 
    // }
    [Fact(Skip = "demo")]
    public void FluentElevatorRequestSystemWorks()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheHelp, () => new TheHelp(new Elevator()))
                .Options(opt => [
                        opt.As("MoveUp").UseThe(Keys.TheHelp)
                            .Now(a => a.Elevator.MoveUp())
                            .Expect("MoveUp Does Not Exceed Max Floor").UseThe(Keys.TheHelp)
                                .Ensure(a => a.Elevator.CurrentFloor <= Elevator.MaxFloor)
                            .Expect("MoveUp Does Not Increment When Doors Are Open").UseThe(Keys.TheHelp)
                                .OnlyWhen(a => a.Tracker.DoorsOpen)
                                .Ensure(a => a.Elevator.CurrentFloor == a.Tracker.CurrentFloor),
                        opt.As("MoveDown").UseThe(Keys.TheHelp)
                            .Now(a => a.Elevator.MoveDown())
                            .Expect("MoveDown Does Not Go Below Ground Floor").UseThe(Keys.TheHelp)
                                .Ensure(a => a.Elevator.CurrentFloor >= 0)
                            .Expect("MoveDown Does Not Decrement When Doors Are Open").UseThe(Keys.TheHelp)
                                .OnlyWhen(a => a.Tracker.DoorsOpen)
                                .Ensure(a => a.Elevator.CurrentFloor == a.Tracker.CurrentFloor),
                        opt.As("OpenDoors").UseThe(Keys.TheHelp).Now(a => a.Elevator.OpenDoors()),
                        opt.As("CloseDoors").UseThe(Keys.TheHelp).Now(a => a.Elevator.CloseDoors())
                    ]).Before(() => { /* need context here*/ })
                .PickOne()
                .DumpItInAcid()
                .AndCheckForGold(30, 10);

        if (report != null)
            Assert.Fail(report.ToString());
    }
}

