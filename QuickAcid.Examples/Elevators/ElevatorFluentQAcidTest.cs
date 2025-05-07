using QuickAcid.Fluent;

namespace QuickAcid.Examples.Elevators;

public static class Keys
{
    public static QKey<Elevator> Elevator =>
        QKey<Elevator>.New("Elevator");
    public static QKey<int> PreviousFloor =>
        QKey<int>.New("PreviousFloor");
}

public static class FluentElevatorHelpers
{
    public static Elevator Elevator(this QAcidContext ctx)
        => ctx.Get(Keys.Elevator);
    public static int PreviousFloor(this QAcidContext ctx)
        => ctx.Get(Keys.PreviousFloor);
}

public class ElevatorFluentQAcidTest
{
    [Fact]
    public void FluentElevatorRequestSystemWorks()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.Elevator, () => new Elevator())
                .Capture(Keys.PreviousFloor, ctx => ctx.Elevator().CurrentFloor)
                .Options(opt => [
                    opt.As("MoveUp").UseThe(Keys.Elevator).Now(e => e.MoveUp())
                        .Expect("MoveUp Does Not Exceed Max Floor")
                            .UseThe(Keys.Elevator)
                            .Ensure(e => e.CurrentFloor <= Elevator.MaxFloor)
                        .Expect("MoveUp Does Not Increment When Doors Are Open")
                            .OnlyWhen(ctx => ctx.Elevator().DoorsOpen)
                            .Ensure(ctx => ctx.Elevator().CurrentFloor == ctx.PreviousFloor()),
                    opt.As("MoveDown").UseThe(Keys.Elevator).Now(e => e.MoveDown())
                        .Expect("MoveDown Does Not Go Below Ground Floor")
                            .Ensure(ctx => ctx.Elevator().CurrentFloor >= 0)
                        .Expect("MoveDown Does Not Decrement When Doors Are Open")
                            .OnlyWhen(ctx => ctx.Elevator().DoorsOpen)
                            .Ensure(ctx => ctx.Elevator().CurrentFloor == ctx.PreviousFloor()),
                    opt.As("OpenDoors").UseThe(Keys.Elevator).Now(e => e.OpenDoors()),
                    opt.As("CloseDoors").UseThe(Keys.Elevator).Now(e => e.CloseDoors())
                ])
                .DumpItInAcid()
                .AndCheckForGold(30, 10);

        if (report != null)
            Assert.Fail(report.ToString());
    }
}

