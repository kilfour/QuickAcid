using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;
using Jint;
using QuickPulse.Instruments;
using Jint.Native;
using Jint.Runtime;

namespace QuickAcid.Mocha.Tests.CoinChallenge;

public class CoinChallengeTest
{
    [Fact]
    public void AcidTest()
    {
        var path = SolutionLocator.FindSolutionRoot() + "\\QuickAcid.Mocha.Tests";
        var module = From.Path(path).AndFile("./CoinChallenge/coin-challenge.js");
        var script =

            from amount in "amount".Input(MGen.Int(-1, 20))
            from coins in "coins".Input(MGen.Int(-1, 11).Many(0, 5))


            from result in "minCoins".Act(
                () => module.Call("minCoins", amount, module.CreateJsArray([.. coins])))
            let number = result.AsNumber()

            from resultS in "minCoinsS".Act(
                () => module.Call("minCoins", amount, module.CreateJsArray(coins.OrderBy(_ => Guid.NewGuid()).ToArray())))
            let numberS = resultS.AsNumber()

            from resultR in "minCoinsR".Act(
                () => module.Call("minCoins", amount, module.CreateJsArray(coins.Reverse().ToArray())))
            let numberR = resultR.AsNumber()

            from _neg in "negative amount returns Infinity".SpecIf(() => amount < 0, () => double.IsPositiveInfinity(number))

            from _s0 in "minCoins(0, any) == 0".SpecIf(() => amount == 0, () => number == 0)

            from _s1 in "if not 'Infinity' return value should be zero or greater".SpecIf(
                () => !double.IsPositiveInfinity(number),
                () => number >= 0)

            from _s2 in "adding a useless coin to the tail should not change result".SpecIf(
                () => !double.IsPositiveInfinity(number),
                () =>
                {
                    var newCoins = coins.Concat([amount + 1]).ToArray();
                    var newResult = module.Call("minCoins", amount, module.CreateJsArray(newCoins));
                    return number == newResult.AsNumber();
                })

            from _s3 in "adding a useless coin to the head should not change result".SpecIf(
                () => !double.IsPositiveInfinity(number),
                () =>
                {
                    var newCoins = new[] { amount + 1 }.Concat(coins).ToArray();
                    var newResult = module.Call("minCoins", amount, module.CreateJsArray(newCoins));
                    return number == newResult.AsNumber();
                })

            from _s4 in "reversed coins should not change result".SpecIf(
                () => !double.IsPositiveInfinity(number),
                () => number == numberR)

            from _s5 in "shuffling coins should not change result".SpecIf(
                () => !double.IsPositiveInfinity(number),
                () => number == numberS)

            select Acid.Test;
        5000.Times(() => new QState(script).Testify(1));
    }
}


public static class JintInterop
{
    public static JsValue CreateJsArray<T>(Engine engine, T[] items, Func<T, JsValue> toJsValue)
    {
        var jsArray = engine.Intrinsics.Array.Construct(Arguments.Empty);
        for (uint i = 0; i < items.Length; i++)
        {
            jsArray.Set(i, toJsValue(items[i]), throwOnError: false);
        }
        return jsArray;
    }
}