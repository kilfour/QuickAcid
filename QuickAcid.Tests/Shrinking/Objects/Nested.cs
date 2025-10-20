using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class Nested
    {
        [Fact]
        public void OneRelevantProperty()
        {
            var script =
                from input in "input".Input(
                    from _ in Fuzzr.Constant("Answer me").Replace()
                    from __ in Fuzzr.Constant(42).Replace()
                    from thing in Fuzzr.One<Parent>()
                    select thing)
                from act in "act".Act(() => { })
                from spec in "spec".SpecIf(() => input.MyChild != null, () => input.MyChild!.MyInt != 42)
                select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run(script)
                .WithOneRun()
                .And(50.ExecutionsPerRun()));

            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Label);
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