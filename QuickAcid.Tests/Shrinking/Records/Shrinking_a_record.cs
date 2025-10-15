using QuickAcid.Tests._Tools;
using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class Shrinking_a_record
    {
        public record Person(string Name, int Age);

        [Fact]
        public void One_record()
        {
            var generator =
                from name in Fuzz.String()
                from age in Fuzz.Constant(42)
                select new Person(name, age);

            var script =
                from input in "input".Input(generator)
                from foo in "act".Act(() => { if (input.Age == 42) throw new Exception(); })
                select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run(script)
                .WithOneRun()
                .AndOneExecutionPerRun());

            Assert.Equal(1, article.Total().Inputs());

            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Label);
            Assert.Equal("{ Age : 42 }", inputEntry.Value);

            var actEntry = article.Execution(1).Action(1).Read();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Label);
            Assert.NotNull(article.Exception());
        }

        [Fact]
        public void Shrinker_cannot_shrink_relevant_field_in_record_YES_IT_CAN()
        {
            var generator1 =
                from name in Fuzz.Constant("any")
                from age in Fuzz.Constant(43)
                select new Person(name, age);

            var generator2 =
                from name in Fuzz.Constant("any")
                from age in Fuzz.Constant(40)
                select new Person(name, age);

            var script =
                from input1 in "input1".Input(generator1)
                from input2 in "input2".Input(generator2)
                from foo in "act".Act(() =>
                {
                    if (input1.Age <= 42 || input2.Age <= 42) throw new Exception();
                })
                select Acid.Test;

            var article = TheJournalist.Exposes(() => QState.Run(script)
                .WithOneRun()
                .AndOneExecutionPerRun());

            var inputEntry = article.Execution(1).Input(1).Read();
            Assert.NotNull(inputEntry);

            Assert.Equal("input2", inputEntry.Label);
            Assert.Equal("{ Age : 40 }", inputEntry.Value);
        }

        [Fact]
        public void Shrinker_cannot_mutate_Age_in_record_YES_IT_CAN()
        {

            100.Times(() =>
            {
                var generator =
                from name in Fuzz.Constant("any")
                from age in Fuzz.ChooseFromThese(40, 41, 42, 1000)
                select new Person(name, age);

                var script =
                    from input in "input".Input(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.Age < 43) throw new Exception();
                    })
                    select Acid.Test;

                var article = TheJournalist.Exposes(() => QState.Run(script)
                    .WithOneRun()
                    .And(30.ExecutionsPerRun()));

                var inputEntry = article.Execution(1).Input(1).Read();
                Assert.NotNull(inputEntry);
                Assert.NotEqual("Age : 1000", inputEntry.Value);
            });
        }
    }
}