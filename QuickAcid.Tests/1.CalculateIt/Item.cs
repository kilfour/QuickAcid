namespace LegacyLogic;

public class Item
{
    public string? Name { get; set; }
    public string? Category { get; set; }
    public decimal Cost { get; set; }
    public int Number { get; set; }
    public bool Import { get; set; }
    public decimal Rate { get; set; }

    public override string ToString()
    {
        return $"{{ Name: {Name}, Category: {Category}, Cost: {Cost}, Number: {Number}, Import: {Import}, Rate: {Rate} }}";
    }
}
