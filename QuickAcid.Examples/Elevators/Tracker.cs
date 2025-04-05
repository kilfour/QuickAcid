namespace QuickAcid.Examples.Elevators;

public class Tracker
{
    public int CurrentFloor;
    public bool DoorsOpen;
    public List<int> Requests = new();
    public List<int> ServedRequests = new();
    public int OperationsPerformed;

    public Tracker(Elevator elevator)
    {
        Do(elevator);
    }

    public void Do(Elevator elevator)
    {
        CurrentFloor = elevator.CurrentFloor;
        DoorsOpen = elevator.DoorsOpen;
        Requests = elevator.Requests.ToList(); // <-- Snapshot taken here
        OperationsPerformed++;

        if (DoorsOpen && Requests.Contains(CurrentFloor))
            ServedRequests.Add(CurrentFloor);
    }
}
