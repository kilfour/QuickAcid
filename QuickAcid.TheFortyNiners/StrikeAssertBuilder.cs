
using QuickMGenerate.UnderTheHood;
using QuickAcid.Fluent;
using QuickAcid.Bolts;

namespace QuickAcid.TheFortyNiners;

public class StrikeAssertBuilder<TModel>
{
    private readonly Func<TModel> modelFactory;
    private readonly Func<TModel, string> stringify;
    private readonly (string name, Generator<object> generator)[] fuzzers;
    private readonly (string name, Delegate action)[] operations;
    private readonly List<(string, Func<TModel, bool>)> checks = new();

    public StrikeAssertBuilder(
        Func<TModel> modelFactory,
        Func<TModel, string> stringify,
        (string name, Generator<object> generator)[] fuzzers,
        (string name, Delegate action)[] operations)
    {
        this.modelFactory = modelFactory;
        this.fuzzers = fuzzers;
        this.operations = operations;
        this.stringify = stringify;
    }

    public StrikeAssertBuilder<TModel> Assert(string label, Func<TModel, bool> invariant)
    {
        checks.Add((label, invariant));
        return this;
    }

    public void UnitRun()
    {
        Run(1, 1);
    }

    private Bob CallBob()
    {
        if (stringify == null)
            return SystemSpecs.Define().AlwaysReported("Model", modelFactory);
        return SystemSpecs.Define().AlwaysReported("Model", modelFactory, stringify);
    }

    public void Run(int scopes, int executionsPerScope)
    {
        var systemSpecs = CallBob(); // --> defines AlwaysReported once

        systemSpecs = systemSpecs.Options(opt =>
            operations.Select(op =>
            {
                var sub = opt
                    .Fuzzed(op.name, FindFuzzer(op.name)) // ✅ Only Fuzzed per op
                    .Do(op.name, ctx =>
                    {
                        var model = ctx.GetItAtYourOwnRisk<TModel>("Model");
                        var input = ctx.GetItAtYourOwnRisk<object>(op.name);

                        op.action.DynamicInvoke(model, input);
                    });

                return sub;
            }));

        foreach (var check in checks)
        {
            systemSpecs = systemSpecs.Assert(check.Item1, ctx =>
            {
                var model = ctx.GetItAtYourOwnRisk<TModel>("Model");
                return check.Item2(model);
            });
        }

        systemSpecs
           .DumpItInAcid()
           .ThrowFalsifiableExceptionIfFailed(scopes, executionsPerScope);
    }



    private Generator<object> FindFuzzer(string name)
    {
        foreach (var (fuzzName, generator) in fuzzers)
        {
            if (fuzzName == name)
                return generator;
        }
        throw new InvalidOperationException($"No fuzzer registered for input '{name}'");
    }
}
