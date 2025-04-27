using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Strike;

public class StrikeActBuilder<TModel>
{
    private readonly Func<TModel> modelFactory;
    private readonly Func<TModel, string> stringify;
    private readonly (string name, Generator<object> generator)[] fuzzers;

    public StrikeActBuilder(Func<TModel> modelFactory, Func<TModel, string> stringify, (string name, Generator<object> generator)[] fuzzers)
    {
        this.modelFactory = modelFactory;
        this.stringify = stringify;
        this.fuzzers = fuzzers;
    }

    public StrikeAssertBuilder<TModel> Act(params (string name, Delegate action)[] operations)
    {
        return new StrikeAssertBuilder<TModel>(modelFactory, stringify, fuzzers, operations);
    }
}