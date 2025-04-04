namespace QuickAcid.Examples.Elevators;

public class Elevator
{
    public int CurrentFloor = 0;
    public bool DoorsOpen = false;

    public const int MaxFloor = 10;

    public void MoveUp()
    {
        if (!DoorsOpen && CurrentFloor < MaxFloor)
            CurrentFloor++;
    }

    public void MoveDown()
    {
        if (!DoorsOpen && CurrentFloor > 0)
            CurrentFloor--;
    }

    public void OpenDoors() => DoorsOpen = true;
    public void CloseDoors() => DoorsOpen = false;
}