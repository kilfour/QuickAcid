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

    [Fact(Skip = "Only for demo")]
    public void FluentElevatorRequestSystemWorks()
    {
        var elevator = new Elevator();
        var tracker = new Tracker(elevator);

        var report =
        SystemSpecs.Define()
            .Tracked(Keys.TheHelp, () => new TheHelp(new Elevator()))
            .Options(opt => [
                    opt.Do("MoveUp", ctx => () => ctx.Get(Keys.TheHelp).Elevator.MoveUp())
                        .Expect("MoveUp Does Not Exceed Max Floor")
                            .Ensure(() => elevator.CurrentFloor <= Elevator.MaxFloor)
                        .Expect("MoveDown Does Not Decrement When Doors Are Open")
                            .OnlyWhen(() => tracker.DoorsOpen)
                            .Ensure(() => elevator.CurrentFloor == tracker.CurrentFloor),
                    opt.Do("MoveDown", () => elevator.MoveDown()),
                    opt.Do("OpenDoors", () => elevator.OpenDoors()),
                    opt.Do("CloseDoors", () => elevator.CloseDoors())
                ])
            .PickOne()
            .DumpItInAcid()
            .AndCheckForGold(30, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}

