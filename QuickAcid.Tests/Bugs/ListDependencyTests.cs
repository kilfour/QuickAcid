using QuickFuzzr;
using QuickPulse.Bolts;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Bugs;


public class ListDependencyTests
{
    [Fact(Skip = "exploring")]
    public void Simple()
    {
        var script =
            from boxOne in "box 1".Stashed(() => new Box<List<string>>([]))
            from boxTwo in "box 2".Stashed(() => new Box<List<string>>([]))
            from boxOneContent in "box 1 content".Input(Fuzz.Constant(new List<string> { "one" }))
            from boxTwoContent in "box 2 content".Input(Fuzz.Constant(new List<string> { "two" }))
            from boxOneAdd in "box 1 add".Act(() => boxOne.Value = boxOneContent)
            from boxTwoAdd in "box 2 add".Act(() => boxTwo.Value = boxTwoContent)
            from spec in "spec".Spec(() => boxOne.Value.All(a => boxTwo.Value.Contains(a)))
            select Acid.Test;

        QState.Run(script).WithOneRunAndOneExecution();
    }

    [Fact(Skip = "exploring")]
    public void NotSoSimple()
    {
        var script =
            from boxOne in "box 1".Stashed(() => new Box<List<string>>([]))
            from boxTwo in "box 2".Stashed(() => new Box<List<string>>([]))
            from connected in "connected".Stashed(() => new Box<bool>(false))
            from listWithA in "setup value".Input(Fuzz.Constant(new List<string> { "A" }))
            from listWithB in "box 2 change value".Input(Fuzz.Constant(new List<string> { "B" }))
            from seq in Script.Choose(
                from a in "set up boxes".ActIf(
                    () => boxOne.Value.Count == 0,
                    () => { boxOne.Value = listWithA; boxTwo.Value = listWithA; })
                select Acid.Test,
                from b in "set connected to true".ActIf(
                    () => boxOne.Value.Count > 0 && boxOne.Value.All(a => boxTwo.Value.Contains(a)),
                    () => connected.Value = true)
                select Acid.Test,
                from c in "box 2 change".ActIf(
                    () => connected.Value,
                    () => boxTwo.Value = listWithB)
                select Acid.Test)
            from spec in "spec".Spec(() => boxOne.Value.All(a => boxTwo.Value.Contains(a)))
            select Acid.Test;

        QState.Run("not-so-simple", script)
            //.Options(a => a with { Verbose = true })
            .WithOneRun()
            .And(30.ExecutionsPerRun());
    }

    [Fact(Skip = "exploring")]
    public void NotSoSimpleChooseIf()
    {
        var script =
            from boxOne in "box 1".Stashed(() => new Box<List<string>>([]))
            from boxTwo in "box 2".Stashed(() => new Box<List<string>>([]))
            from connected in "connected".Stashed(() => new Box<bool>(false))
            from listWithA in "setup value".Input(Fuzz.Constant(new List<string> { "A" }))
            from listWithB in "box 2 change value".Input(Fuzz.Constant(new List<string> { "B" }))
            from seq in Script.ChooseIf(
                (() => boxOne.Value.Count == 0,
                    "set up boxes".Act(() => { boxOne.Value = listWithA; boxTwo.Value = listWithA; })),
                (() => boxOne.Value.Count > 0 && boxOne.Value.All(a => boxTwo.Value.Contains(a)),
                    "set connected to true".Act(() => { connected.Value = true; })),
                (() => connected.Value,
                    "box 2 change".Act(() => { boxTwo.Value = listWithB; })))
            from spec in "spec".Spec(() => boxOne.Value.All(a => boxTwo.Value.Contains(a)))
            select Acid.Test;

        QState.Run("not-so-simple-choose-if", script)
            //.Options(a => a with { Verbose = true })
            .WithOneRun()
            .And(30.ExecutionsPerRun());
    }
}