using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class Nested
    {
        [Fact]
        public void OneRelevantProperty()
        {
            var script =
                from input in "input".Input(
                    from _ in Fuzz.Constant("Answer me").Replace()
                    from __ in Fuzz.Constant(42).Replace()
                    from thing in Fuzz.One<Parent>()
                    select thing)
                from act in "act".Act(() => { })
                from spec in "spec".SpecIf(() => input.MyChild != null, () => input.MyChild!.MyInt != 42)
                select Acid.Test;

            var report = new QState(script).Observe(50);
            Assert.NotNull(report);
            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("{ MyChild : { MyInt : 42 } }", inputEntry.Value);
        }


        public class Parent
        {
            public Child? MyChild { get; set; }
        }

        public class Child
        {
            public int MyInt { get; set; }
            public string? MyIrrelevantString { get; set; }
        }
    }
}