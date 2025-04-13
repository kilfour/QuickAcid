namespace QuickAcid.Examples.Tutorial.Chapter1.YourFirstPropertyTest;

public class Counter
{
    private int _value;

    public int Value => _value;

    public void Increment() => _value++;
    public void Decrement()
    {
        // 🐞 Try removing the condition below to introduce a bug:
        // The counter should never go negative.
        if (_value > 0)
            _value--;
    }
}