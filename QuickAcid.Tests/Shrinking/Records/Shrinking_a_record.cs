using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Shrinking.Objects
{
    public class Shrinking_a_record
    {
        public record Person(string Name, int Age);

        [Fact]
        public void One_record()
        {
            var generator =
                from name in MGen.String()
                from age in MGen.Constant(42)
                select new Person(name, age);

            var script =
                from input in "input".Input(generator)
                from foo in "act".Act(() => { if (input.Age == 42) throw new Exception(); })
                select Acid.Test;

            var report = new QState(script).ObserveOnce();

            Assert.Single(report.OfType<ReportInputEntry>());

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input", inputEntry.Key);
            Assert.Equal("{ Age : 42 }", inputEntry.Value);

            var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(report.Exception);
        }

        [Fact]
        public void Shrinker_cannot_shrink_relevant_field_in_record_YES_IT_CAN()
        {
            var generator1 =
                from name in MGen.Constant("any")
                from age in MGen.Constant(43)
                select new Person(name, age);

            var generator2 =
                from name in MGen.Constant("any")
                from age in MGen.Constant(40)
                select new Person(name, age);

            var script =
                from input1 in "input1".Input(generator1)
                from input2 in "input2".Input(generator2)
                from foo in "act".Act(() =>
                {
                    if (input1.Age <= 42 || input2.Age <= 42) throw new Exception();
                })
                select Acid.Test;

            var report = new QState(script).ObserveOnce();

            var inputEntry = report.Single<ReportInputEntry>();
            Assert.NotNull(inputEntry);

            Assert.Equal("input2", inputEntry.Key);
            Assert.Equal("{ Age : 40 }", inputEntry.Value);
        }

        [Fact]
        public void Shrinker_cannot_mutate_Age_in_record_YES_IT_CAN()
        {

            100.Times(() =>
            {
                var generator =
                from name in MGen.Constant("any")
                from age in MGen.ChooseFromThese(40, 41, 42, 1000)
                select new Person(name, age);

                var script =
                    from input in "input".Input(generator)
                    from foo in "act".Act(() =>
                    {
                        if (input.Age < 43) throw new Exception();
                    })
                    select Acid.Test;

                var report = new QState(script).Observe(30);

                var inputEntry = report.Single<ReportInputEntry>();
                Assert.NotNull(inputEntry);
                Assert.NotEqual("Age : 1000", inputEntry.Value);
            });
        }
    }
}