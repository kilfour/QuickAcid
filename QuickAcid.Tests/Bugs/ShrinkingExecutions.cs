// using QuickAcid.Reporting;
// using QuickFuzzr;

// namespace QuickAcid.Tests.Bugs;

// public class ShrinkingExecutions
// {
//     public class Holder
//     {
//         public List<Person> Collection { get; } = [];
//     }

//     public class Person
//     {
//         public Person(int id) { Id = id; }
//         public int Id { get; }
//         public int Count { get; private set; } = 0;
//         public void DoCount() { Count++; }
//     }

//     [Fact]
//     public void Trying_To_Find_Failure()
//     {
//         var script =
//             from holder in "container".Stashed(() => new Holder())
//             from ops in "ops".Choose(
//                 from id in "to add".Input(Fuzz.Int(1, 10).Unique("id"))
//                 from adding in "adding".Act(() => holder.Collection.Add(new Person(id)))
//                 select Acid.Test,
//                 from getPerson in "getPerson".Act()
//                 from counting in "counting".Act(() =>
//                 { if (true) { throw new Exception(); } })
//                 select Acid.Test
//             )
//             select Acid.Test;
//         var ex = Assert.Throws<FalsifiableException>(() =>
//             QState.Run(script)
//                 .WithOneRun()
//                 .And(100.ExecutionsPerRun()));
//         var report = ex.QAcidReport;
//         Assert.Equal(2, report.OfType<ReportInputEntry>().Count());
//         var entry1 = report.First<ReportInputEntry>();
//         Assert.Equal("i1", entry1.Key);
//         Assert.Equal("42", entry1.Value);
//         var entry2 = report.Second<ReportInputEntry>();
//         Assert.Equal("i2", entry2.Key);
//         Assert.Equal("42", entry2.Value);
//     }
// }