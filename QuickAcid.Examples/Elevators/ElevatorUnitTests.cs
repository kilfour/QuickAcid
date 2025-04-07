namespace QuickAcid.Examples.Elevators;

public class ElevatorUnitTests
{
    [Fact]
    public void InitialFloor_IsZero()
    {
        var elevator = new Elevator();
        Assert.Equal(0, elevator.CurrentFloor);
    }

    [Fact]
    public void Fail()
    {
        var elevator = new Elevator();
        elevator.CloseDoors();
        elevator.MoveDown();
        Assert.Equal(0, elevator.CurrentFloor);
    }

    [Fact]
    public void MoveUp_IncrementsFloor_WhenDoorsClosed()
    {
        var elevator = new Elevator();
        elevator.CloseDoors();
        elevator.MoveUp();
        Assert.Equal(1, elevator.CurrentFloor);
    }

    [Fact]
    public void MoveUp_DoesNotIncrement_WhenDoorsOpen()
    {
        var elevator = new Elevator();
        elevator.OpenDoors();
        elevator.MoveUp();
        Assert.Equal(0, elevator.CurrentFloor);
    }

    [Fact]
    public void MoveDown_DecrementsFloor_WhenDoorsClosed()
    {
        var elevator = new Elevator();
        elevator.CloseDoors();
        elevator.MoveUp(); // now floor 1
        elevator.MoveDown();
        Assert.Equal(0, elevator.CurrentFloor);
    }

    [Fact]
    public void MoveDown_DoesNotDecrement_WhenAtGroundFloor()
    {
        var elevator = new Elevator();
        elevator.CloseDoors();
        elevator.MoveDown();
        Assert.Equal(0, elevator.CurrentFloor);
    }

    [Fact]
    public void MoveDown_DoesNotDecrement_WhenDoorsOpen()
    {
        var elevator = new Elevator();
        elevator.CloseDoors();
        elevator.MoveUp(); // floor 1
        elevator.OpenDoors();
        elevator.MoveDown(); // shouldn't move
        Assert.Equal(1, elevator.CurrentFloor);
    }
}
