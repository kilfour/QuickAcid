using QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Reporting;
using QuickMGenerate;
using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace LegacyLogic;

public class AcidTests
{
    [Fact(Skip = "Trove of goodies, a.k.a. hard stuff to shrink")]
    public void ModelTestingReport()
    {
        Report report = null!;
        try
        {
            report =
                QState.Run(TheScript)
                    .With(500.Runs())
                    .AndOneExecutionPerRun();
            Signal.Tracing<string>().SetArtery(new WriteDataToFile("model-testing.log"))
                .Pulse(report.Entries.Select(a => a.ToString()!));
        }
        catch (FalsifiableException ex)
        {
            report = ex.QAcidReport;
            Signal.From(filterTraces).SetArtery(new WriteDataToFile("model-testing.log").ClearFile())
                .Pulse(report.ShrinkTraces);
            throw;
        }
    }

    private static readonly QAcidScript<Acid> TheScript =
        from _ in Acid.Script
            // Arrange
        let categories = (string[])["toys", "Eco", "other", "cooking utensils", "clothes", "snacks"]
        let rates = (decimal[])[0.06m, 0.12m, 0.21m]
        from sCategory in Shrink<Item>.For(a => a.Category, _ => categories)
        from sRate in Shrink<Item>.For(a => a.Rate, _ => rates)
        let itemGenerator =
            from _1 in MGen.For<Item>().Customize(a => a.Category, MGen.ChooseFromThese(categories))
            from _2 in MGen.For<Item>().Customize(a => a.Cost, MGen.Decimal().Apply(d => Math.Round(d, 2)))
            from _3 in MGen.For<Item>().Customize(a => a.Rate, MGen.ChooseFromThese(rates))
            from item in MGen.One<Item>()
            select item
        from items in "items".Input(itemGenerator.Many(1, 1))
            // Act
        from baseline in "baseline stashed".Stashed(() => new CalculatorOld())
        from baselineResult in "Baseline".Act(() => baseline.Total([.. items]))
        from variant in "variant stashed".Stashed(() => new Calculator())
        from variantResult in "Variant".Act(() => variant.Total([.. items]))
            // Assert
        from bogo in "bogo".Act(() => items.Any(a => a.Category == "toys" && a.Number > 1))
        from bogoDiscount in "BOGO Discount".SpecIf(() => bogo, () => baselineResult != variantResult)
        from eco in "eco".Act(() => items.Any(a => a.Category == "Eco" && a.Cost < 20 && a.Import == false))
        from ecoSubsidyAgrees in "Eco Subsidy".SpecIf(() => eco, () => baselineResult != variantResult)
        from agreesWhen in "agreesWhen".Act(() => !bogo && !eco)
        from agrees in "Variant Matches Baseline".SpecIf(() => agreesWhen, () => baselineResult == variantResult)
        select Acid.Test;

    private Flow<ShrinkTrace> filterTracesOld =
        from input in Pulse.Start<ShrinkTrace>()
        let isToy = input.Original?.ToString() == "toys"
        from trace in Pulse.Trace(input)
        from traceIf in Pulse.TraceIf(isToy, $" =====>  {input}")
        select input;

    private Flow<ShrinkTrace> filterTraces =
        from input in Pulse.Start<ShrinkTrace>()
        from _ in Pulse.TraceIf(input.Strategy == "PrimitiveShrink" && input.Intent == ShrinkIntent.Keep,
            $"{input.Key}: {input.Intent}, {input.Strategy} ({QuickAcidStringify.Default()(input.Result!)})")
        from __ in Pulse.TraceIf(input.Strategy == "PrimitiveShrink" && input.Intent != ShrinkIntent.Keep,
            $"{input.Key}: {input.Intent}, {input.Strategy}")
        from ___ in Pulse.TraceIf(input.Strategy != "PrimitiveShrink",
            $"{input.Key}: {input.Intent}, {input.Strategy}")
        select input;
}