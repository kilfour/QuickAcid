using QuickAcid.Bolts.Nuts;
using QuickMGenerate;
using Jint;
using QuickPulse.Instruments;
using QuickPulse.Arteries;


namespace QuickAcid.Mocha.Tests.CoinChallenge;

public class CoinChallengeTest
{
    private static ModuleWrapper GetModule()
    {
        var path = SolutionLocator.FindSolutionRoot() + "\\QuickAcid.Mocha.Tests\\CoinChallenge\\";
        // var file = "./coin-challenge.js";
        // var file = "./coin-challenge-bugged.js";
        var file = "./coin-challenge-not-minimal.js";
        return From.Path(path).AndFile(file);
    }

    private static double CallMinCoins(ModuleWrapper module, int amount, int[] coins)
    {
        return module.Call("minCoins", amount, module.CreateJsArray([.. coins])).AsNumber();
    }

    [Fact]
    public void AcidTest()
    {
        var writer = new WriteDataToFile().ClearFile();
        var fastTestsOnly = false;
        var module = GetModule();
        var script =
            from amount in "amount".Input(MGen.Int(-1, 20))
            from coins in "coins".Input(MGen.Int(-1, 11).Many(0, 5))
            from result in "minCoins".Act(() => CallMinCoins(module, amount, [.. coins]))
            from trace in "minCoins result".Trace($"{result}")

            from _ in GeneralProperties(amount, result)
            from __ in UselessCoins(module, amount, [.. coins], result)
            from ___ in ReverseCoins(module, amount, [.. coins], result)
            from ____ in IsOptimal(amount, [.. coins], result).SkipIf(() => fastTestsOnly)
            select Acid.Test;
        1000.Times(() => Assert.Null(new QState(script).ObserveOnce()));
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


    // Adding coin '1' to coins => newResult <= result 
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

    private static QAcidScript<Acid> ReverseCoins(ModuleWrapper module, int amount, int[] coins, double result)
    {

        return
            from _ in Acid.Script
            let notInfinity = !double.IsPositiveInfinity(result)
            let reversedCoins = notInfinity ? (int[])[.. coins.Reverse()] : []
            let reversedResult = notInfinity ? CallMinCoins(module, amount, reversedCoins) : -1
            from reversed in "reversed coins should not change result".DelayedSpecIf(
                () => notInfinity, () => result == reversedResult)
            from trace1 in "original coins".TraceIf(
                () => reversed.Failed,
                $"[ {string.Join(", ", coins)} ] => {result}")
            from trace2 in "reversed coins".TraceIf(
                () => reversed.Failed,
                $"[ {string.Join(", ", reversedCoins)} ] => {reversedResult}")
            let apply = reversed.Apply()
            select Acid.Test;
    }

    private static QAcidScript<Acid> IsOptimal(int amount, int[] coins, double result)
    {
        var writer = new WriteDataToFile();
        return
            from _ in Acid.Script
            let optimal = amount >= 0 && coins.All(c => c > 0) ? Optimal(amount, coins) : int.MaxValue
            from ds in
            "result should be minimal compared to known optimal".DelayedSpecIf(
                () => amount >= 0 && coins.All(c => c > 0),
                () => Matches(result, optimal))
            from o in "original coins".TraceIf(() => ds.Failed, $"[ {string.Join(", ", coins)} ]")
            from t in "trace optimal".TraceIf(() => ds.Failed, $"{result} : {optimal}")
            let apply = ds.Apply()
            select Acid.Test;
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