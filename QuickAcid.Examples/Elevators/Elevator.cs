namespace QuickAcid.Examples.Elevators;

public class Elevator
{
    public int CurrentFloor = 0;
    public bool DoorsOpen = true;
    public const int MaxFloor = 10;
    public IEnumerable<int> Requests => _requests;
    private readonly Queue<int> _requests = new();

    public void Call(int floor)
    {
        if (floor >= 0 && floor <= MaxFloor)
            _requests.Enqueue(floor);
    }

    public void Step()
    {
        if (_requests.Count == 0 || DoorsOpen)
            return;

        var target = _requests.Peek();
        if (CurrentFloor < target) MoveUp();
        else if (CurrentFloor > target) MoveDown();
        else OpenDoors(); // we've arrived
    }

    public void MoveUp()
    {
        if (!DoorsOpen && CurrentFloor < MaxFloor)
            CurrentFloor++;
    }

    public void MoveDown()
    {
        if (!DoorsOpen && CurrentFloor > 0) // change to >= for bug
            CurrentFloor--;
    }

    public void OpenDoors()
    {
        DoorsOpen = true;
        if (_requests.Count == 0)
        {
            // comment return out for bug
            return;
        }
        _requests.Dequeue(); // assume we served the request
    }

    public void CloseDoors() => DoorsOpen = false;

    public bool HasRequests => _requests.Count > 0;
    public int? NextRequest => _requests.Count > 0 ? _requests.Peek() : null;
}
