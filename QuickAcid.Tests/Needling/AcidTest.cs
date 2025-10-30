using QuickFuzzr;
using QuickPulse.Bolts;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Needling;

public class AcidTest
{
    public static async Task<Cell<int>> GetAnswerAsync(int a) => await Task.FromResult(new Cell<int>(a));

    [Fact]
    public void Spike()
    {
        var script =
            from needler in "Needler".Stashed(() => new Needler<int, Cell<int>>())

            from key in "key".Input(Fuzzr.Guid().AsString())
            from input in "input".Input(Fuzzr.Int())
            from start in "Start".Act(() => GetAnswerAsync(input).Attach(needler, key, input))

            from toReload in $"To Reload".Input(Fuzzr.OneOfOrDefault(needler.Keys))
            from check in "IsIdentity".SpecIf(
                () => needler.HasDataWaiting,
                () => needler.Check(input => input, output => output.Value))
            select Acid.Test;

        QState.Run(script)
            .Options(a => a with { FileAs = "Spike" })
            .WithOneRun()
            .And(5.ExecutionsPerRun());
    }
}
