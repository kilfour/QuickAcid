using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Tests.Shrinking.Enumerables;

public class Shrinking_a_list_of_records
{
    public record Person(string Name, int Age);

    [Fact]
    public void Two_records()
    {
        var counter = 41;
        var generator =
            from name in MGen.Constant("jos")
            from age in MGen.Constant(counter++)
            select new Person(name, age);

        var script =
            from input in "input".Input(generator.Many(2))
            from foo in "act".Act(() =>
            {
                if (input.Any(a => a.Age == 42)) { throw new Exception(); }
            })
            select Acid.Test;

        var report = new QState(script).ObserveOnce();

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Key);
        Assert.Equal("[ { Age : 42 } ]", inputEntry.Value);

        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Key);
        Assert.NotNull(report.Exception);
    }


    [Fact]
    public void Clone_List_Of_Persons_And_Cast_Back_Successfully()
    {
        // Arrange
        var original = new List<Person>
        {
            new Person("Alice", 30),
            new Person("Bob", 40)
        };

        // Act
        var cloned = EnumerableShrinkStrategy.CloneAsOriginalTypeList(original);

        // Assert
        var casted = (IEnumerable<Person>)cloned; // ✅ Should NOT throw

        Assert.Collection(casted,
            p => Assert.Equal("Alice", p.Name),
            p => Assert.Equal("Bob", p.Name)
        );
    }

    [Fact]
    public void Clone_Array_Of_Persons_And_Cast_Back_Successfully()
    {
        // Arrange
        Person[] original =
        {
            new Person("Alice", 30),
            new Person("Bob", 40)
        };

        // Act
        var cloned = (object)EnumerableShrinkStrategy.CloneAsOriginalTypeList((object)original);

        // Assert
        var casted = (IEnumerable<Person>)cloned; // ✅ Should NOT throw

        Assert.Collection(casted,
            p => Assert.Equal("Alice", p.Name),
            p => Assert.Equal("Bob", p.Name)
        );
    }
}