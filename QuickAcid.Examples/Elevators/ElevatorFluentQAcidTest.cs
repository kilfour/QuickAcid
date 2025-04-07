using QuickAcid.Fluent;

namespace QuickAcid.Examples.Elevators;

public class ElevatorFluentQAcidTest : QAcidLoggingFixture
{
    public static class Keys
    {
        public static QKey<Elevator> Elevator =>
            QKey<Elevator>.New("Elevator");
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
                .AlwaysReported(Keys.Elevator, () => new Elevator())
                .Options(opt => [
                        opt.As("MoveUp").UseThe(Keys.Elevator)
                            .Now(a => a.MoveUp())
                            .Expect("MoveUp Does Not Exceed Max Floor").UseThe(Keys.Elevator)
                                .Ensure(a => a.CurrentFloor <= Elevator.MaxFloor)
                            .Expect("MoveUp Does Not Increment When Doors Are Open").UseThe(Keys.Elevator)
                                .OnlyWhen(a => a.DoorsOpen)
                                .Ensure(a => a.CurrentFloor == a.CurrentFloor),
                        opt.As("MoveDown").UseThe(Keys.Elevator)
                            .Now(a => a.MoveDown())
                            .Expect("MoveDown Does Not Go Below Ground Floor").UseThe(Keys.Elevator)
                                .Ensure(a => a.CurrentFloor >= 0)
                            .Expect("MoveDown Does Not Decrement When Doors Are Open").UseThe(Keys.Elevator)
                                .OnlyWhen(a => a.DoorsOpen)
                                .Ensure(a => a.CurrentFloor == a.CurrentFloor),
                        opt.As("OpenDoors").UseThe(Keys.Elevator).Now(a => a.OpenDoors()),
                        opt.As("CloseDoors").UseThe(Keys.Elevator).Now(a => a.CloseDoors())
                    ])
                .PickOne()
                .DumpItInAcid()
                .AndCheckForGold(30, 10);

        if (report != null)
            Assert.Fail(report.ToString());
    }
}

