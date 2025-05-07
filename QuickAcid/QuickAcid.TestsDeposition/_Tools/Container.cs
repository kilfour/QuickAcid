namespace QuickAcid.TestsDeposition._Tools;

public class Container<T>
{
    public T? Value { get; set; }
    public Container() { }
    public Container(T? value) { Value = value; }
}
