using QuickAcid.Bolts.ShrinkStrats;
using QuickMGenerate;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Tests.Shrinking;

public class CompositeGeneratorSpike
{
    public class Composed
    {
        public int One { get; set; }
        public int Two { get; set; }
    }

    [Fact(Skip = "WIP")]
    public void Initial()
    {
        var script =
            from inputOne in "inputOne".Input(MGen.Constant(42))
            from inputTwo in "inputTwo".Input(MGen.Constant(0))
            from inputThree in "inputThree".Input(MGen.Constant(0))
            from composed in "composed".Input(MGen.Constant(new Composed() { One = inputOne, Two = inputTwo }))
            from spec in "spec".Spec(() => composed.One + composed.Two != 42)
            select Acid.Test;
        new QState(script).TestifyOnce();
        // var report = new QState(script).ObserveOnce();
        // Signal.Tracing<ShrinkTrace>().SetArtery(new WriteDataToFile("CompositeGeneratorSpike1.log").ClearFile()).Pulse(report.ShrinkTraces);
    }

    [Fact(Skip = "WIP")]
    public void Using_Derived()
    {
        var script =
            from inputOne in "inputOne".Input(MGen.Constant(21))
            from inputTwo in "inputTwo".Input(MGen.Constant(21))
            from composed in "composed".Derived(MGen.Constant(new Composed() { One = inputOne, Two = inputTwo }))
            from spec in "spec".Spec(() => composed.One + composed.Two != 42)
            select Acid.Test;
        //new QState(script).TestifyOnce();
        var report = new QState(script).ObserveOnce();
        //Signal.Tracing<ShrinkTrace>().SetArtery(new WriteDataToFile().ClearFile()).Pulse(report.ShrinkTraces);
        Signal.Tracing<string>().SetArtery(new WriteDataToFile().ClearFile()).Pulse(report.Entries.Select(a => a.ToString())!);
    }

    [Fact(Skip = "WIP")]
    public void Desired_Result()
    {
        // MGen.For<SomeThingToGenerate>().Customize(s => s.MyProperty, MGen.Constant(42))
        var script =
            from composed in "composed".Composed(
                from inputOne in "inputOne".Input(MGen.Constant(42))
                from inputTwo in "inputTwo".Input(MGen.Constant(0))
                select new Composed() { One = inputOne, Two = inputTwo })
            from trace in "trace".Trace(() => "should be: - Input : composed = { One : 42, Two : 0 }")
            from spec in "spec".Spec(() => composed.One + composed.Two != 42)
            select Acid.Test;

        // new QState(script).TestifyOnce();
        var report = new QState(script).ObserveOnce();
        // Signal.Tracing<ShrinkTrace>().SetArtery(new WriteDataToFile("CompositeGeneratorSpike2.log").ClearFile()).Pulse(report.ShrinkTraces);
        Signal.Tracing<string>().SetArtery(new WriteDataToFile().ClearFile()).Pulse(report.Entries.Select(a => a.ToString())!);
    }
}