namespace QuickAcid.Examples.Elevators;

public class Tracker
{
    public int CurrentFloor;
    public bool DoorsOpen = true;
    public int OperationsPerformed;
    public Tracker(Elevator elevator)
    {
        Do(elevator);
    }
    public void Do(Elevator elevator)
    {
        CurrentFloor = elevator.CurrentFloor;
        DoorsOpen = elevator.DoorsOpen;
        OperationsPerformed++;
    }

    public override string ToString() =>
        $"CurrentFloor: {CurrentFloor}, DoorsOpen: {DoorsOpen}, Ops: {OperationsPerformed}";
}
