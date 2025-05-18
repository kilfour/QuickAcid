using QuickAcid.Bolts.Nuts;
using QuickMGenerate;
using Jint;
using QuickPulse.Instruments;


namespace QuickAcid.Mocha.Tests.CoinChallenge;

public class CoinChallengeTest
{
    private static ModuleWrapper GetModule()
    {
        var path = SolutionLocator.FindSolutionRoot() + "\\QuickAcid.Mocha.Tests\\CoinChallenge\\";
        var file = "./coin-challenge.js";
        // var file = "./coin-challenge-bugged.js";
        // var file = "./coin-challenge-not-minimal.js";
        return From.Path(path).AndFile(file);
    }

    private static double CallMinCoins(ModuleWrapper module, int amount, int[] coins)
    {
        return module.Call("minCoins", amount, module.CreateJsArray([.. coins])).AsNumber();
    }

    [Fact]
    public void AcidTest()
    {
        var fastTestsOnly = true;
        var module = GetModule();
        var script =
            from amount in "amount".Input(MGen.Int(-1, 20))
            from coins in "coins".Input(MGen.Int(-1, 11).Many(0, 5))
            from result in "minCoins".Act(() => CallMinCoins(module, amount, [.. coins]))
            from trace in "minCoins result".Trace(result.ToString())
            from _ in GeneralProperties(amount, result)
            from __ in UselessCoins(module, amount, [.. coins], result)
            from ___ in Permutations(module, amount, [.. coins], result)
            from ____ in IsOptimal(amount, [.. coins], result).SkipIf(() => fastTestsOnly)
            select Acid.Test;
        100.Times(() => Assert.Null(new QState(script).ObserveOnce()));
    }

    private static QAcidScript<Acid> GeneralProperties(int amount, double result)
    {
        return
            from _neg in "negative amount returns Infinity".SpecIf(
                () => amount < 0, () => double.IsPositiveInfinity(result))
            from _s0 in "minCoins(0, any) == 0".SpecIf(
                () => amount == 0, () => result == 0)
            from _s1 in "if not 'Infinity' return value should be zero or greater".SpecIf(
                () => !double.IsPositiveInfinity(result),
                () => result >= 0)
            select Acid.Test;
    }

    private static QAcidScript<Acid> UselessCoins(ModuleWrapper module, int amount, int[] coins, double result)
    {
        return
            from _ in "adding a useless coin to the head should not change result".SpecIf(
                () => !double.IsPositiveInfinity(result),
                () => result == CallMinCoins(module, amount, [amount + 1, .. coins]))
            from __ in "adding a useless coin to the tail should not change result".SpecIf(
                () => !double.IsPositiveInfinity(result),
                () => result == CallMinCoins(module, amount, [.. coins, amount + 1]))
            select Acid.Test;
    }

    private static QAcidScript<Acid> Permutations(ModuleWrapper module, int amount, int[] coins, double result)
    {
        return
            from _ in "reversed coins should not change result".SpecIf(
                () => !double.IsPositiveInfinity(result),
                () => result == CallMinCoins(module, amount, [.. coins.Reverse()]))
            from __ in "shuffling coins should not change result".SpecIf(
                () => !double.IsPositiveInfinity(result),
                () => result == CallMinCoins(module, amount, [.. coins.OrderBy(_ => Guid.NewGuid())]))
            select Acid.Test;
    }

    private static QAcidScript<Acid> IsOptimal(int amount, int[] coins, double result)
    {
        return
            "result should be minimal compared to known optimal".SpecIf(
                () => amount >= 0 && coins.All(c => c > 0),
                () => Matches(result, Optimal(amount, coins)));
    }

    private static bool Matches(double jsResult, int optimal)
    {
        return double.IsPositiveInfinity(jsResult)
            ? optimal == int.MaxValue
            : jsResult == optimal;
    }

    private static int Optimal(int amount, int[] coins)
    {
        var memo = new Dictionary<int, int>();

        int Dp(int amt)
        {
            if (amt < 0) return int.MaxValue;
            if (amt == 0) return 0;
            if (memo.TryGetValue(amt, out var val)) return val;

            int best = int.MaxValue;
            foreach (var c in coins)
            {
                int sub = Dp(amt - c);
                if (sub != int.MaxValue)
                    best = Math.Min(best, sub + 1);
            }

            memo[amt] = best;
            return best;
        }

        var r = Dp(amount);
        return r == int.MaxValue ? int.MaxValue : r;
    }
}